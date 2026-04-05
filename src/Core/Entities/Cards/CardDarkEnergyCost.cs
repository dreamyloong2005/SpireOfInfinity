using MegaCrit.Sts2.Core.Entities.Cards;
using SpireOfInfinity.Core.Extensions;
using SpireOfInfinity.Core.Models.Cards;
using SpireOfInfinity.Core.Models.Powers;

namespace SpireOfInfinity.Core.Entities.Cards
{
    public sealed class DarkEnergyCost
    {
        private readonly WodBaseCard _card;
        private int _base;
        private int _capturedXValue;
        private List<LocalCostModifier> _localModifiers = [];

        public int Canonical { get; }
        public bool CostsX { get; }
        public bool DynamicCost { get; }
        public bool WasJustUpgraded { get; private set; }
        public bool HasLocalModifiers => _localModifiers.Count > 0;

        public int CapturedXValue
        {
            get
            {
                // if (!CostsX)
                //     throw new InvalidOperationException("Only X-cost cards have a captured value.");
                return _capturedXValue;
            }
            set
            {
                _card.AssertMutable();
                // if (!CostsX)
                //     throw new InvalidOperationException("Only X-cost cards have a captured value.");
                _capturedXValue = value;
            }
        }

        public DarkEnergyCost(WodBaseCard card, int canonicalCost, bool costsX, bool dynamicCost)
        {
            _card = card;
            CostsX = costsX;
            DynamicCost = dynamicCost;
            Canonical = costsX ? 0 : canonicalCost;
            _base = Canonical;
        }

        public int GetWithModifiers(CostModifiers modifiers)
        {
            int num = _base;
            if (_card.IsCanonical) return num;
            if (_base < 0) return num;
            if (CostsX) return num;

            if (modifiers.HasFlag(CostModifiers.Local))
            {
                foreach (var mod in _localModifiers)
                    num = mod.Modify(num);
            }

            if (modifiers.HasFlag(CostModifiers.Global) && _card.CombatState != null)
            {
            }

            return Math.Max(0, num);
        }

        public int GetAmountToSpend()
        {
            if(_card.Owner.Creature.HasPower<DarknessAwakenPower>())
            {
                return 0;
            }
            if (CostsX)
            {
                return _card.Owner?.PlayerCombatState?.GetData()?.DarkEnergy ?? 0;
            }
            if (DynamicCost)
            {
                if(_card.Owner.PlayerCombatState.GetData().DarkEnergy >= GetWithModifiers(CostModifiers.None))
                {
                    return Math.Max(0, GetWithModifiers(CostModifiers.None));
                }else
                {
                    return 0;
                }
            }
            return Math.Max(0, GetWithModifiers(CostModifiers.None));
        }

        public int GetResolved()
        {
            if (CostsX) return CapturedXValue;
            return Math.Max(0, GetWithModifiers(CostModifiers.None));
        }

        public void SetUntilPlayed(int cost, bool reduceOnly = false)
        {
            if (cost != 0 || Canonical >= 0)
                _localModifiers.Add(new LocalCostModifier(cost, LocalCostType.Absolute, LocalCostModifierExpiration.WhenPlayed, reduceOnly));
        }

        public void SetThisTurnOrUntilPlayed(int cost, bool reduceOnly = false)
        {
            if (cost != 0 || Canonical >= 0)
                _localModifiers.Add(new LocalCostModifier(cost, LocalCostType.Absolute, LocalCostModifierExpiration.EndOfTurn | LocalCostModifierExpiration.WhenPlayed, reduceOnly));
        }

        public void SetThisTurn(int cost, bool reduceOnly = false)
        {
            if (cost != 0 || Canonical >= 0)
                _localModifiers.Add(new LocalCostModifier(cost, LocalCostType.Absolute, LocalCostModifierExpiration.EndOfTurn, reduceOnly));
        }

        public void SetThisCombat(int cost, bool reduceOnly = false)
        {
            if (cost != 0 || Canonical >= 0)
                _localModifiers.Add(new LocalCostModifier(cost, LocalCostType.Absolute, LocalCostModifierExpiration.EndOfCombat, reduceOnly));
        }

        public void AddUntilPlayed(int amount, bool reduceOnly = false)
        {
            if (amount != 0)
                _localModifiers.Add(new LocalCostModifier(amount, LocalCostType.Relative, LocalCostModifierExpiration.WhenPlayed, reduceOnly));
        }

        public void AddThisTurnOrUntilPlayed(int amount, bool reduceOnly = false)
        {
            if (amount != 0)
                _localModifiers.Add(new LocalCostModifier(amount, LocalCostType.Relative, LocalCostModifierExpiration.EndOfTurn | LocalCostModifierExpiration.WhenPlayed, reduceOnly));
        }

        public void AddThisTurn(int amount, bool reduceOnly = false)
        {
            if (amount != 0)
                _localModifiers.Add(new LocalCostModifier(amount, LocalCostType.Relative, LocalCostModifierExpiration.EndOfTurn, reduceOnly));
        }

        public void AddThisCombat(int amount, bool reduceOnly = false)
        {
            if (amount != 0)
                _localModifiers.Add(new LocalCostModifier(amount, LocalCostType.Relative, LocalCostModifierExpiration.EndOfCombat, reduceOnly));
        }

        public bool EndOfTurnCleanup()
        {
            _card.AssertMutable();
            return _localModifiers.RemoveAll(m => m.Expiration.HasFlag(LocalCostModifierExpiration.EndOfTurn)) > 0;
        }

        public bool AfterCardPlayedCleanup()
        {
            _card.AssertMutable();
            return _localModifiers.RemoveAll(m => m.Expiration.HasFlag(LocalCostModifierExpiration.WhenPlayed)) > 0;
        }

        public void UpgradeBy(int addend)
        {
            _card.AssertMutable();
            if (CostsX || addend == 0) return;

            int oldBase = _base;
            int newBase = Math.Max(_base + addend, 0);
            WasJustUpgraded = true;

            if (newBase < oldBase)
            {
                foreach (var mod in _localModifiers)
                {
                    if (mod.Type == LocalCostType.Absolute && mod.Amount > newBase)
                        mod.Amount = newBase;
                }
            }
            SetCustomBaseCost(newBase);
        }

        public void FinalizeUpgrade()
        {
            _card.AssertMutable();
            WasJustUpgraded = false;
        }

        public void ResetForDowngrade()
        {
            _card.AssertMutable();
            _base = Canonical;
            _card.InvokeDarkEnergyCostChanged();
        }

        public void SetCustomBaseCost(int newBaseCost)
        {
            _card.AssertMutable();
            _base = newBaseCost;
            _card.InvokeDarkEnergyCostChanged();
        }

        public DarkEnergyCost Clone(WodBaseCard newCard)
        {
            var clone = new DarkEnergyCost(newCard, newCard.CanonicalDarkEnergyCost, newCard.HasDarkEnergyCostX, newCard.DynamicDarkEnergyCost)
            {
                _base = _base,
                _capturedXValue = _capturedXValue,
                WasJustUpgraded = WasJustUpgraded,
                _localModifiers = [.. _localModifiers]
            };
            return clone;
        }
    }
}
