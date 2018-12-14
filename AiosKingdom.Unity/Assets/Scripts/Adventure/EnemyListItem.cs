using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnemyListItem : MonoBehaviour
{
    public GameObject Selected;
    public Text Name;
    public Text Health;
    public Button Action;

    public void Initialize(JsonObjects.AdventureState.EnemyState enemyState)
    {
        var monster = DatasManager.Instance.Monsters.FirstOrDefault(m => m.Id.Equals(enemyState.MonsterId));

        Name.text = string.Format("{0} {1}", enemyState.EnemyType, monster.Name);
        Health.text = string.Format("[{0}/{1}]", enemyState.State.CurrentHealth, enemyState.State.MaxHealth);
    }

    public void Select()
    {
        Selected.SetActive(true);
    }

    public void Unselect()
    {
        Selected.SetActive(false);
    }

    private void SelectEnemy(JsonObjects.AdventureState.EnemyState enemy)
    {
        /*
        var monster = DatasManager.Instance.Monsters.FirstOrDefault(m => m.Id.Equals(enemy.MonsterId));
        var skill = DatasManager.Instance.Books.FirstOrDefault(b => b.Pages.Any(p => p.Id.Equals(monster.Phases[enemy.NextPhase].SkillId)));

        EnemyBox.SetActive(true);
        EnemyHealth.text = string.Format("[{0}/{1}]", enemy.State.CurrentHealth, enemy.State.MaxHealth);
        EnemyDamages.text = string.Format("[{0}-{1}]", enemy.State.MinDamages, enemy.State.MaxDamages);
        EnemyPhase.text = skill.Name;
        */
    }
}
