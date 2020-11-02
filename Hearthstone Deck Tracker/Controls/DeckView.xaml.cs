﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HearthDb.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Utility;
using Hearthstone_Deck_Tracker.Utility.Extensions;
using static HearthDb.CardIds.Collectible;
using static System.Windows.Visibility;

namespace Hearthstone_Deck_Tracker.Controls
{
	public partial class DeckView
	{
		private readonly string _allTags;

		public DeckView(Deck deck, bool deckOnly = false)
		{
			InitializeComponent();
			_allTags = deck.TagList.ToLowerInvariant().Replace("-", "");
			ListViewPlayer.Update(deck.Cards.ToSortedCardList(), true);

			if(deckOnly)
			{
				DeckTitleContainer.Visibility = Collapsed;
				DeckFormatPanel.Visibility = Collapsed;
				SetDustPanel.Visibility = Collapsed;
				BrandContainer.Visibility = Collapsed;
			}
			else
			{
				DeckTitlePanel.Background = DeckHeaderBackground(deck.Class);
				LblDeckTitle.Text = deck.Name;
				LblDeckTag.Text = GetTagText(deck);
				LblDeckFormat.Text = GetFormatText(deck);
				LblDustCost.Text = TotalDust(deck).ToString();
				ShowFormatIcon(deck);
				SetIcons.Update(deck);
			}
		}

		private ImageBrush DeckHeaderBackground(string deckClass)
		{
			var heroId = ClassToID(deckClass);
			var drawingGroup = new DrawingGroup();
			var img = ImageCache.GetCardImage(Database.GetCardFromId(heroId));
			drawingGroup.Children.Add(new ImageDrawing(img, new Rect(54, 0, 130, 34)));
			drawingGroup.Children.Add(new ImageDrawing(new BitmapImage(new Uri(
				"Images/Themes/Bars/dark/fade.png", UriKind.Relative)), new Rect(0, 0, 183, 34)));

			return new ImageBrush {
				ImageSource = new DrawingImage(drawingGroup),
				AlignmentX = AlignmentX.Left,
				Stretch = Stretch.UniformToFill
			};
		}

		private string GetTagText(Deck deck)
		{
			var predefined = new List<string>() {
				"Midrange",
				"Aggro",
				"Control",
				"Tempo",
				"Combo"
			};

			if(deck.Tags.Count > 0)
				foreach(var tag in predefined)
					if(_allTags.Contains(tag.ToLowerInvariant()))
						return tag;

			return LocUtil.Get(deck.Class);
		}

		private string GetFormatText(Deck deck)
		{
			if(deck.IsArenaDeck)
				return "Arena";
			if(_allTags.Contains("brawl"))
				return "Brawl";
			if(_allTags.Contains("adventure") || _allTags.Contains("pve"))
				return "Adventure";
			if(deck.IsDungeonDeck)
				return "Dungeon";
			if(deck.IsDuelsDeck)
				return "Duels";
			if(deck.StandardViable)
				return "Standard";
			return "Wild";
		}

		private void ShowFormatIcon(Deck deck)
		{
			RectIconStandard.Visibility = Collapsed;
			RectIconWild.Visibility = Collapsed;
			RectIconArena.Visibility = Collapsed;
			RectIconBrawl.Visibility = Collapsed;
			RectIconAdventure.Visibility = Collapsed;

			if(deck.IsArenaDeck)
				RectIconArena.Visibility = Visible;
			else if(_allTags.Contains("brawl"))
				RectIconBrawl.Visibility = Visible;
			else if(_allTags.Contains("adventure") || _allTags.Contains("pve") || deck.IsDungeonDeck)
				RectIconAdventure.Visibility = Visible;
			else if(_allTags.Contains("duels") || deck.IsDuelsDeck)
				RectIconDuels.Visibility = Visible;
			else if(deck.StandardViable)
				RectIconStandard.Visibility = Visible;
			else
				RectIconWild.Visibility = Visible;
		}

		private int TotalDust(Deck deck)
		{
			var nonCraftableSets = new[]
			{
				CardSet.KARA,
				CardSet.NAXX,
				CardSet.BRM,
				CardSet.LOE,
				CardSet.CORE
			}.Select(HearthDbConverter.SetConverter).ToList();
			var nonCraftableCards = new List<string>() {
				Neutral.Cthun,
				Neutral.BeckonerOfEvil
			};

			return deck.Cards
				.Where(c => !nonCraftableSets.Contains(c.Set) && !nonCraftableCards.Contains(c.Id))
				.Sum(c => c.DustCost * c.Count);
		}

		private string ClassToID(string klass)
		{
			switch(klass.ToLowerInvariant())
			{
				case "druid":
					return Druid.MalfurionStormrageHero;
				case "demonhunter":
					return Demonhunter.IllidanStormrageHero;
				case "hunter":
					return Hunter.RexxarHero;
				case "mage":
					return Mage.JainaProudmooreHero;
				case "paladin":
					return Paladin.UtherLightbringerHero;
				case "priest":
					return Priest.AnduinWrynnHero;
				case "rogue":
					return Rogue.ValeeraSanguinarHero;
				case "shaman":
					return Shaman.ThrallHero;
				case "warlock":
					return Warlock.GuldanHero;
				case "warrior":
				default:
					return Warrior.GarroshHellscreamHero;
			}
		}
	}
}
