using System;
using System.Globalization;

public class CityCard : Card
{
    public override void Populate(Card card, QEntity qEntity)
    {
        var qCity = qEntity as QCity;
        var cityCard = card as CityCard;

        if (qCity == null) throw new Exception("QEntity is not of the expected type.");
        if (cityCard == null) throw new Exception("Card is not of the expected type.");

        SetTitle(qCity.id);

        AddPropertyToGroup(propertyGroup1, "Population", qCity.population.ToString(CultureInfo.CurrentCulture));
        AddPropertyToGroup(propertyGroup1, "Population Growth Rate",
            qCity.populationGrowthRate.ToString(CultureInfo.CurrentCulture));
        AddPropertyToGroup(propertyGroup1, "Population Decline Rate",
            qCity.populationDeclineRate.ToString(CultureInfo.CurrentCulture));

        foreach (var resource in qCity.demand)
            AddResourceToGroup(propertyGroup2, resource);
    }
}