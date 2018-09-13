using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer
{
    public static class SkillAndEffect
    {
        public static Network.AdventureState.ActionResult ExecuteInscription(Network.PlayerState from, Network.PlayerState to, 
            DataModels.Skills.Inscription inscription,
            out Network.PlayerState fromOut, out Network.PlayerState toOut)
        {
            fromOut = CopyState(from);
            toOut = CopyState(to);

            Random rand = new Random();
            double amount = inscription.BaseValue + (inscription.Ratio * GetStatValue(inscription.StatType, from));

            Network.AdventureState.ActionResult result = new Network.AdventureState.ActionResult
            {
                // TODO : Find a way for this.
                // TargetId = enemy.MonsterId,
                IsConsumable = false,
                Id = inscription.PageId,
                Amount = amount
            };

            if (inscription.IncludeWeaponDamages)
            {
                if (inscription.WeaponTypes.Where(w => from.WeaponTypes.Contains(w.ToString())).Count() > 0)
                {
                    int wpDmg = rand.Next(from.MinDamages, from.MaxDamages + 1);
                    Console.WriteLine($"IncludeWeaponType += {wpDmg} * {inscription.WeaponDamagesRatio}");
                    amount += (wpDmg * inscription.WeaponDamagesRatio);
                }

                if (inscription.PreferredWeaponTypes.Where(w => from.WeaponTypes.Contains(w.ToString())).Count() > 0)
                {
                    int wpDmg = rand.Next(from.MinDamages, from.MaxDamages + 1);
                    Console.WriteLine($"IncludePreferredWeaponType += {wpDmg} * {inscription.PreferredWeaponDamagesRatio}");
                    amount += (wpDmg * inscription.PreferredWeaponDamagesRatio);
                }
            }

            switch (inscription.Type)
            {
                case DataModels.Skills.InscriptionType.Damages:
                    {
                        toOut.CurrentHealth -= amount;
                        Console.WriteLine($"{from} using skill doing ({inscription.Type}).({inscription.BaseValue}+{inscription.StatType}({GetStatValue(inscription.StatType, from)})*{inscription.Ratio}) to {to}.");

                        result.ResultType = Network.AdventureState.ActionResult.Type.Damage;
                    }
                    break;
                case DataModels.Skills.InscriptionType.Heal:
                    {
                        fromOut.CurrentHealth += amount;
                        if (fromOut.CurrentHealth > fromOut.MaxHealth)
                        {
                            fromOut.CurrentHealth = fromOut.MaxHealth;
                        }
                        Console.WriteLine($"{from} using skill doing ({inscription.Type}).({inscription.BaseValue}+{inscription.StatType}({GetStatValue(inscription.StatType, from)})*{inscription.Ratio}) on himself .");

                        result.ResultType = Network.AdventureState.ActionResult.Type.Heal;
                    }
                    break;
            }

            return result;
        }

        public static Network.AdventureState.ActionResult ExecuteEffects(Network.PlayerState from, Network.PlayerState to, 
            DataModels.Items.ConsumableEffect effect,
            out Network.PlayerState fromOut, out Network.PlayerState toOut)
        {
            fromOut = CopyState(from);
            toOut = CopyState(to);

            Network.AdventureState.ActionResult result = new Network.AdventureState.ActionResult
            {
                IsConsumable = true,
                Id = effect.ConsumableId,
                Amount = effect.AffectValue
            };

            switch (effect.Type)
            {
                case DataModels.Items.EffectType.RestoreHealth:
                    {
                        fromOut.CurrentHealth += effect.AffectValue;
                        if (fromOut.CurrentHealth > fromOut.MaxHealth)
                        {
                            fromOut.CurrentHealth = fromOut.MaxHealth;
                        }
                        Console.WriteLine($"Using consumable doing ({effect.Type}).({effect.AffectValue}) on yourself .");

                        result.ResultType = Network.AdventureState.ActionResult.Type.Heal;
                    }
                    break;
                case DataModels.Items.EffectType.ResoreMana:
                    {
                        fromOut.CurrentMana += effect.AffectValue;
                        if (fromOut.CurrentMana > fromOut.MaxMana)
                        {
                            fromOut.CurrentMana = fromOut.MaxMana;
                        }
                        Console.WriteLine($"Using consumable doing ({effect.Type}).({effect.AffectValue}) on yourself .");

                        result.ResultType = Network.AdventureState.ActionResult.Type.ReceiveMana;
                    }
                    break;
            }

            return result;
        }

        public static int GetStatValue(DataModels.Soul.Stats stat, Network.PlayerState state)
        {
            switch (stat)
            {
                case DataModels.Soul.Stats.Stamina:
                    return state.Stamina;
                case DataModels.Soul.Stats.Energy:
                    return state.Energy;
                case DataModels.Soul.Stats.Strength:
                    return state.Strength;
                case DataModels.Soul.Stats.Agility:
                    return state.Agility;
                case DataModels.Soul.Stats.Intelligence:
                    return state.Intelligence;
                case DataModels.Soul.Stats.Wisdom:
                    return state.Wisdom;
                default:
                    return 0;
            }
        }

        private static Network.PlayerState CopyState(Network.PlayerState state)
        {
            if (state == null)
                return null;

            return new Network.PlayerState
            {
                MaxHealth = state.MaxHealth,
                MaxMana = state.MaxMana,

                CurrentHealth = state.CurrentHealth,
                CurrentMana = state.CurrentMana,

                Stamina = state.Stamina,
                Energy = state.Energy,
                Strength = state.Strength,
                Agility = state.Agility,
                Intelligence = state.Intelligence,
                Wisdom = state.Wisdom,

                WeaponTypes = state.WeaponTypes,
                MinDamages = state.MinDamages,
                MaxDamages = state.MaxDamages
            };
        }
    }
}
