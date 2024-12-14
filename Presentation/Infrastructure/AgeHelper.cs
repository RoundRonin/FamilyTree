namespace FamilyTreeBlazor.presentation.Infrastructure;

public static class AgeHelper
{
    public static int GetAgeFromBirthDay(DateTime dateTime)
    {
        return DateTime.Now.Year - dateTime.Year;
    }
}
