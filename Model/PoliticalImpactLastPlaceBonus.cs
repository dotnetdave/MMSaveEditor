﻿public class PoliticalImpactLastPlaceBonus : PoliticalImpact
{
    public bool active;

    public PoliticalImpactLastPlaceBonus(string inName, string inEffect)
    {
        if (inEffect == null)
            return;

        switch (inName)
        {
            case "Active":
                this.active = true;
                break;
            case "Inactive":
                this.active = false;
                break;
        }
    }

    public override void SetImpact(ChampionshipRules inRules)
    {
        inRules.LastPlaceBonus = this.active;
    }
}
