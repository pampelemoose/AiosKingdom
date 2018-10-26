using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AiosKingdom.ViewModels
{
    public class DungeonListPageViewModel : BaseViewModel
    {
        public DungeonListPageViewModel(INavigation nav)
            : base(nav)
        {
            Title = "Dungeons";

            MessagingCenter.Subscribe<NetworkManager>(this, MessengerCodes.DungeonListUpdated, (sender) =>
            {
                NotifyPropertyChanged(nameof(Dungeons));
            });

            NetworkManager.Instance.AskKnowledges();
        }

        public List<Network.Adventures.Dungeon> Dungeons
        {
            get
            {
                if (DatasManager.Instance.Datas == null || DatasManager.Instance.Dungeons == null)
                    return new List<Network.Adventures.Dungeon>();

                return DatasManager.Instance.Dungeons.Where(d => d.RequiredLevel <= DatasManager.Instance.Datas.Level && d.MaxLevelAuthorized >= DatasManager.Instance.Datas.Level).ToList();
            }
        }

        public Network.Adventures.Dungeon SelectedDungeon
        {
            get { return null; }
            set
            {
                if (value != null)
                {
                    _navigation.PushModalAsync(new Views.Dungeon.EnterDungeonPage(value));
                }

                value = null;
                NotifyPropertyChanged();
            }
        }
    }
}
