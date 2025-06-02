namespace Common.Static;

public static class RandomDateTime
{
    public static DateTime Minutes()
    {
        return Minutes(new DateTime(2002, 2, 2), DateTime.UtcNow);
    }

    public static DateTime Minutes(DateTime start, DateTime end, Random rand = null)
    {
        rand ??= new Random();
        var delta = new TimeSpan(0, rand.Next(0, (int)(end - start).TotalMinutes), 0);

        return start + delta;
    }
}
