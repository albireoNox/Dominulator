﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Generic = System.Collections.Generic;
using System.ComponentModel;

namespace Win8Client
{
    public sealed partial class MainPage : Page
    {
        AppDataContext appDataContext;

        public MainPage()
        {
            this.appDataContext = new AppDataContext(this);  

            this.InitializeComponent();
                  
            this.DataContext = this.appDataContext;
            var uiScheduler = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();

            this.CurrentCards.CurrentCardsChanged += UpdateAllCardsListSelection;
            this.appDataContext.StrategyReport.PropertyChanged += StrategyReport_PropertyChanged;
            this.appDataContext.PageConfig.PropertyChanged += PageConfig_PropertyChanged;

            System.Threading.Tasks.Task.WhenAll(
                appDataContext.AllCards.Populate(),
                appDataContext.CommonCards.PopulateCommon()
                ).ContinueWith(delegate(System.Threading.Tasks.Task task)
                    {                        
                        this.CurrentCards.Randomize10Cards();                 
                    }, uiScheduler);

            this.Loaded += MainPage_Loaded;
        }

        void PageConfig_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.appDataContext.PageConfig.Value == PageConfig.StrategyReport)
            {
                this.appDataContext.SettingsButtonVisibility.Value = SettingsButtonVisibility.Back;
            }
        }
        
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateAllCardsListSelection();
        }
        
        internal void UpdateAllCardsListSelection()
        {
            this.appDataContext.isCurrentDeckIgnoringAllDeckSelectionUpdates = true;
            this.AllCards.SelectedItems.Clear();
            foreach (DominionCard card in this.appDataContext.CurrentDeck.CurrentCards)
            {
                this.AllCards.SelectedItems.Add(card);
            }
            this.appDataContext.isCurrentDeckIgnoringAllDeckSelectionUpdates = false;
        }
              
      
        internal static void Generate10Random(IList<DominionCard> resultList, IList<DominionCard> sourceList, IList<DominionCard> allCards, IList<DominionCard> itemsToReplace)
        {
            bool isReplacingItems = itemsToReplace != null && itemsToReplace.Count > 0 && sourceList.Count <= 10;
            bool isReducingItems = itemsToReplace != null && itemsToReplace.Count > 0 && sourceList.Count > 10;
            var cardPicker = new UniqueCardPicker(allCards);

            if (isReplacingItems)
            {
                cardPicker.ExcludeCards(itemsToReplace);

                if (itemsToReplace != null)
                {
                    foreach (DominionCard cardToReplace in itemsToReplace)
                    {
                        for (int i = 0; i < resultList.Count; ++i)
                        {
                            if (resultList[i] == cardToReplace)
                            {
                                var nextCard = cardPicker.GetCard(c => true);
                                if (nextCard == null)
                                {
                                    resultList.Remove(cardToReplace);
                                    i--;  // do this index again
                                }
                                else
                                {
                                    resultList[i] = nextCard;
                                }
                            }
                        }
                    }
                }
            }
            else if (sourceList.Count < 10)
            {
                var listRemoved = new List<DominionCard>();
                foreach(var card in resultList)
                {
                    if (!sourceList.Contains(card))
                        listRemoved.Add(card);
                }

                foreach(var card in listRemoved)
                {
                    resultList.Remove(card);
                }                       
            }
            else if (isReducingItems)
            {
                foreach (var card in itemsToReplace)
                {
                    resultList.Remove(card);
                }
            }
            else
            {
                resultList.Clear();
            }

            
            while (resultList.Count < 10)
            {
                DominionCard currentCard = cardPicker.GetCard(c => true);
                if (currentCard == null)
                    break;
                resultList.Add(currentCard);
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            this.appDataContext.CardVisibility.Value = CardVisibility.Settings;
            this.appDataContext.SettingsButtonVisibility.Value = SettingsButtonVisibility.Back;
        }

        private void SettingsBackButton_Click(object sender, RoutedEventArgs e)
        {
            this.appDataContext.CardVisibility.Value = CardVisibility.Current;
            this.appDataContext.SettingsButtonVisibility.Value = SettingsButtonVisibility.Settings;
            this.appDataContext.PageConfig.Value = PageConfig.Design;
        }

        private void AllCardsButton_Click(object sender, RoutedEventArgs e)
        {            
            this.appDataContext.CardVisibility.Value =
                this.appDataContext.CardVisibility.Value == CardVisibility.All ? CardVisibility.Current: CardVisibility.All;
        }

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            this.appDataContext.PageConfig.Value = PageConfig.StrategyReport;            
        }

        void StrategyReport_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.ResultsWebView.NavigateToString(this.appDataContext.StrategyReport.Value);            
        }

    }

    class SortableCardList
        : DependencyObject
    {        
        System.Collections.Generic.List<DominionCard> originalCards;
        private System.Collections.ObjectModel.ObservableCollection<DominionCard> cards;
        private Func<DominionCard, bool> filter;
        private Func<DominionCard, object> keySelector;
        
        public DependencyObjectDecl<string, DefaultEmptyString> CurrentSort { get; private set;}

        public event PropertyChangedEventHandler PropertyChanged;

        public System.Collections.ObjectModel.ObservableCollection<DominionCard> Cards
        {
            get
            {
                return this.cards;
            }
        }

        public Generic.IEnumerable<DominionCard> CurrentCards
        {
            get
            {
                return this.originalCards.Where(this.filter).OrderBy<DominionCard, object>(this.keySelector);
            }
        }

        public SortableCardList()
        {
            this.originalCards = new List<DominionCard>();
            this.cards = new System.Collections.ObjectModel.ObservableCollection<DominionCard>();
            this.filter = delegate(DominionCard card)
            {
                return true;
            };
            this.CurrentSort = new DependencyObjectDecl<string, DefaultEmptyString>(this);
            ClearSort();
        }

        private void ClearSort()
        {
            DefaultOrderNoSort();
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => this.CurrentSort.Value = "Not Sorted");            
        }

        public void DefaultOrderNoSort()
        {
            this.keySelector = delegate(DominionCard card)
            {
                return 0;
            };
        }
     
        public void SortByName()
        {
            SortCards(card => card.Name);
            this.CurrentSort.Value = "By Name";
        }

        public void SortByCost()
        {
            SortCards(card => card.Coin);
            this.CurrentSort.Value = "By Cost";
        }

        public void SortByExpansion()
        {
            SortCards(card => card.Expansion);
            this.CurrentSort.Value = "By Expansion";
        }

        public void ApplyFilter(Func<DominionCard, bool> filter)
        {
            this.filter = filter;
        }

        public System.Threading.Tasks.Task UpdateUI()
        {
            return this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                UpdateUIFromUIThread();
            }).AsTask();
        }

        public void UpdateUIFromUIThread()
        {
            this.cards.Clear();
            foreach (var item in this.CurrentCards)
            {
                this.cards.Add(item);
            }

            if (this.PropertyChanged != null)
                this.PropertyChanged(this, null);
        }

        private void SortCards(Func<DominionCard, object> keySelector)
        {
            this.keySelector = keySelector;
            this.UpdateUI();
        }

        public System.Threading.Tasks.Task Populate()
        {
            return PopulateFromResources().ContinueWith(async (continuation) =>
            {
                await this.UpdateUI();
            });
        }

        public System.Threading.Tasks.Task PopulateCommon()
        {
            return PopulateCommonFromResources().ContinueWith(async (continuation) =>
            {
                await this.UpdateUI();
            });
        }

        private async System.Threading.Tasks.Task PopulateFromResources()
        {
            foreach (Dominion.Card card in Dominion.Cards.AllKingdomCards())
            {
                this.originalCards.Add(DominionCard.Create(card));
            }
        }

        private async System.Threading.Tasks.Task PopulateCommonFromResources()
        {
            Dominion.Card[] commonCards = new Dominion.Card[] {
                Dominion.Cards.Province,
                Dominion.Cards.Duchy,
                Dominion.Cards.Estate,
                Dominion.Cards.Gold,
                Dominion.Cards.Silver,
                Dominion.Cards.Copper,
            };
            foreach (Dominion.Card card in commonCards)
            {
                this.originalCards.Add(DominionCard.Create(card));
            }
        }

        private async System.Threading.Tasks.Task PopulateFromWeb()
        {
            var client = new DominulatorWebClient();
            var allCards = client.GetAllCards();

            await allCards.ContinueWith(async (continuation) =>
            {
                var jsonObject = continuation.Result;
                if (jsonObject == null)
                    return;
                var jsonArray = jsonObject.GetArray();
                if (jsonArray == null)
                    return;


                this.originalCards.Clear();
                foreach (Windows.Data.Json.JsonValue currentItem in jsonArray)
                {
                    this.originalCards.Add(new DominionCard(currentItem));
                }               
            }
            );
        }

        public void Generate10Random(IList<DominionCard> allCards, IList<DominionCard> itemsToReplace)
        {
            MainPage.Generate10Random(this.originalCards, this.Cards, allCards, itemsToReplace);
            this.UpdateUIFromUIThread();
        }       

        public void UpdateOriginalCards(IEnumerable<DominionCard> addedCards, IEnumerable<DominionCard> removedCards)
        {
            foreach(var card in addedCards)
            {
                this.originalCards.Add(card);
            }

            foreach (var card in removedCards)
            {
                this.originalCards.Remove(card);
            }

            this.UpdateUIFromUIThread();
        }
    }    
   
    enum ExpansionIndex
    {
        Alchemy = 0,
        Base = 1,
        Cornucopia = 2,
        DarkAges = 3,
        Guilds = 4,
        Hinterlands = 5,
        Intrigue = 6,
        Promo = 7,
        Prosperity = 8,
        Seaside = 9,
        _Unknown = 10,
        _Count = 11
    }

    class Expansion         
    {
        public string Name { get; private set; }
        public ExpansionIndex Index { get; private set; }
        private DependencyObjectDecl<bool, DefaultTrue> isEnabled;        

        public Expansion(string name, ExpansionIndex index)
        {
            this.Name = name;
            this.Index = index;
            this.isEnabled = new DependencyObjectDecl<bool, DefaultTrue>(this);
        }        

        public DependencyObjectDecl<bool, DefaultTrue> IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
        }
    }

    public class ComparisonToIntegerConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var comparison = (Dominion.Strategy.Description.Comparison)value;
            return (int)comparison;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            int intValue = (int)value;
            return (Dominion.Strategy.Description.Comparison)intValue;
        }
    }

    public class CountSourceToIntegerConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var comparison = (Dominion.Strategy.Description.CountSource)value;
            return (int)comparison;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            int intValue = (int)value;
            return (Dominion.Strategy.Description.CountSource)intValue;
        }
    }

    public class BoolToVisibilityConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var boolValue = (bool)value;
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToInVisibilityConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var boolValue = (bool)value;
            return boolValue ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class CurrentCardVisibilityConverter
      : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cardVisibility = (CardVisibility)value;
            return cardVisibility == CardVisibility.Current ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class AllCardVisibilityConverter
      : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cardVisibility = (CardVisibility)value;
            return cardVisibility == CardVisibility.All ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SettingsVisibilityConverter
      : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cardVisibility = (CardVisibility)value;
            return cardVisibility == CardVisibility.Settings ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SettingsButtonVisibilityConverter
      : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cardVisibility = (SettingsButtonVisibility)value;
            return cardVisibility == SettingsButtonVisibility.Settings ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SettingsBackButtonVisibilityConverter
      : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cardVisibility = (SettingsButtonVisibility)value;
            return cardVisibility == SettingsButtonVisibility.Back ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PageConfigDesignVisibilityConverter
      : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var localValue = (PageConfig)value;
            return localValue == PageConfig.Design? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PageConfigStrategyReportVisibilityConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var localValue = (PageConfig)value;
            return localValue == PageConfig.StrategyReport ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}                    
                     
                     
                     
                     
                     
                     
                     