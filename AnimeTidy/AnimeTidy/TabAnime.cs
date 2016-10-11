﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AnimeTidyLib;
using AnimeTidy.Models;
using AnimeTidy.Cores;

namespace AnimeTidy
{
	public partial class TabAnime : UserControl
	{
		public TabAnime()
		{
			InitializeComponent();
			InitModel();
		}

		private AnimeInfo _animeinfo;
		public AnimeInfo AnimeInfo
		{
			get
			{
				return this._animeinfo;
			}
			set
			{
				this._animeinfo = value;
				if (value != null)
					InitAnime();
			}
		}

		// Initalize the Format of the ObjectListView
		private void InitModel()
		{
			if (ObjectListView.IsVistaOrLater)
				this.Font = new Font("msyh", 8);

			this.olvAnime.AddDecoration(new EditingCellBorderDecoration(true));

			TypedObjectListView<Anime> tolv = new TypedObjectListView<Anime>(this.olvAnime);
			tolv.GenerateAspectGetters();

			// Name of Anime
			TypedColumn<Anime> tc = new TypedColumn<Anime>(this.olvColName);
			tc.AspectPutter = (Anime a, object opn) => { a.Name = opn.ToString(); };

			// Schedule of Anime
			tc = new TypedColumn<Anime>(this.olvColAirdate);
			tc.GroupKeyGetter = (Anime a) => a.Year;

			// Type of Anime
			tc = new TypedColumn<Anime>(this.olvColType);
			tc.AspectPutter = (Anime a, object opt) => { a.Type = (MediaType)opt; };
			tc.ImageGetter = (Anime a) =>
			{
				switch (a.Format)
				{
					case MergeFormat.MKV:
						return Properties.Resources.MKV;

					case MergeFormat.MP4:
						return Properties.Resources.MP4;

					case MergeFormat.AVI:
						return Properties.Resources.AVI;

					case MergeFormat.WMV:
						return Properties.Resources.WMV;

					case MergeFormat.M2TS:
						return Properties.Resources.M2TS;

					default:
						return -1;
				}
			};

			// SubTeam of Anime
			tc = new TypedColumn<Anime>(this.olvColSubTeam);
			tc.AspectPutter = (Anime a, object opp) => { a.SubTeam = opp.ToString(); };
			tc.ImageGetter = (Anime a) =>
			{
				switch (a.SubStyle)
				{
					case SubStyle.External:
						return Properties.Resources.External;

					case SubStyle.Sealed:
						return Properties.Resources.Sealed;

					case SubStyle.Embedded:
						return Properties.Resources.Embedded;

					default:
						return -1;
				}
			};

			// Size of Anime
			this.olvColSize.AspectToStringConverter = ots =>
			{
				long ls = (long)ots;

				if (ls == 0L)
					return "-";
				else if (ls >= 1000000000L)
					return String.Format("{0:#,##0.#0} G", ls / 1073741824D);
				else
					return String.Format("{0:#,##0.#0} M", ls / 1048576D);
			};
			this.olvColSize.MakeGroupies(
				new long[] { 5368709120L, 10737418240L },
				new string[] { "0~5 GB", "5~10 GB", ">10 GB" }
				);

			// Store of Anime
			tc = new TypedColumn<Anime>(this.olvColStore);
			tc.AspectPutter = (Anime a, object opg) => { a.Store = (bool)opg; };
			this.olvColStore.Renderer = new MappedImageRenderer(true, Properties.Resources.Accept, false, Properties.Resources.Alert);

			// Enjoy of Anime
			tc = new TypedColumn<Anime>(this.olvColEnjoy);
			tc.AspectPutter = (Anime a, object opv) => { a.Enjoy = (bool)opv; };
			this.olvColEnjoy.Renderer = new MappedImageRenderer(true, Properties.Resources.Smile, false, Properties.Resources.Sad);

			// Grade of Anime
			tc = new TypedColumn<Anime>(this.olvColGrade);
			tc.AspectPutter = (Anime a, object opr) =>
			{
				int onr = (int)opr;
				a.Grade = onr;//onr < 1 ? 1 : onr;
			};
			this.olvColGrade.Renderer = new MultiImageRenderer(Properties.Resources.Diamond, 3, 0, 4);
			this.olvColGrade.MakeGroupies(
				new int[] { 1, 2 },
				new string[] { "Normal", "Nice", "Good" }
				);

			// Note of Anime
			this.olvColNote.AspectToStringConverter = otn => otn.ToString().Replace('\u0002', '\u0020');

			this.olvAnime.UseTranslucentHotItem = true;
			this.olvAnime.UseTranslucentSelection = true;
			//this.olvAnime.HotItemStyle.Overlay = new AnimeViewOverlay();
			this.olvAnime.HotItemStyle = this.olvAnime.HotItemStyle;
			this.olvAnime.PrimarySortColumn = this.olvColTitle;
			this.olvAnime.PrimarySortOrder = SortOrder.Ascending;
		}

		private void InitAnime()
		{
			if (AnimeInfo.AnimeList != null)
				this.olvAnime.SetObjects(AnimeInfo.AnimeList);
		}
	}
}