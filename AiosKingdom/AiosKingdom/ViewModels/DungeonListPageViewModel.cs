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

            NetworkManager.Instance.AskKnowledges();
        }

        public List<DataModels.Dungeons.Dungeon> Dungeons => DatasManager.Instance.Dungeons;

        public DataModels.Dungeons.Dungeon SelectedDungeon
        {
            get { return null; }
            set
            {
                if (value != null)
                {
                    if (value.RequiredLevel > DatasManager.Instance.Datas.Level)
                        ScreenManager.Instance.AlertScreen(value.Name, $"You can't enter here because your level is too low. (Min Level is {value.RequiredLevel})");
                    else if (value.MaxLevelAuthorized < DatasManager.Instance.Datas.Level)
                        ScreenManager.Instance.AlertScreen(value.Name, $"You can't enter here because your level is too high. (Max Level is {value.MaxLevelAuthorized})");
                    else if (DatasManager.Instance.Knowledges.Count <= 0)
                        ScreenManager.Instance.AlertScreen(value.Name, $"You can't enter here because you didn't learn any skills.");
                    else
                        _navigation.PushModalAsync(new Views.Dungeon.EnterDungeonPage(value));
                }

                value = null;
                NotifyPropertyChanged();
            }
        }
    }
}
