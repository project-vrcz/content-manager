namespace VRChatContentManager.Core.Utils;

public static class RandomWordsUtils
{
    private static readonly string[] Terms =
    [
        "Manuka", "Kipfel", "Shinano", "rurune", "Milltina", "mamehinata", "Selestia", "Kikyo", "Sio", "Chocolat",
        "Karin", "Chiffon", "Moe", "Mafuyu", "Maya", "Rusk", "Lasyusha", "Ichigo", "Zome", "Mizuki", "Milfy", "Rindo",
        "Lime", "Eku", "Hakka", "Deltaflair", "Nagi", "Airi", "Lapwing", "Bokusei", "Marycia", "Lumina", "Ash",
        "usasaki", "Yoll", "nemesis", "Nayu", "Sapphy", "Wolferia", "Felis", "Sophina", "Platinum", "Mishe", "Grus",
        "Kokoa", "Koyuki", "Fiona", "Mint", "minahoshi", "Miminoko", "Soraha", "ELusion", "Maki", "Shizuku-san",
        "Imeris", "Leefa", "Liloumois", "ririka", "Scented",
        "Shiratsume", "Milk", "Lunalitt", "Anon", "Kyoko", "Merino", "Kaya", "Cian", "Lzebul", "meiyun", "Iris", "Eyo",
        "Kuronatsu", "Binah", "Flare", "Chalo", "Velle", "Aldina", "Anomea", "Mulicia", "Mariel", "Shuan", "Kyubi",
        "Foshunia", "Tonnerre", "Cornet", "Azuki", "Hamster", "Lazuli", "Nemo", "Ukon", "Chise"
    ];

    private static readonly string[] Adjectives =
    [
        "Swift", "Silent", "Brave", "Clever", "Mighty", "Fierce", "Gentle", "Nimble", "Bold", "Wise",
        "Loyal", "Radiant", "Vibrant", "Serene", "Daring", "Fearless", "Graceful", "Sturdy", "Valiant", "Zesty",
        "Luminous", "Majestic", "Noble", "Playful", "Quirky", "Resilient", "Sleek", "Tenacious", "Vivid", "Witty",
        "Zealous", "Adventurous", "Brisk", "Charming", "Dazzling", "Eager", "Funky", "Gleaming", "Humble", "Jovial"
    ];

    public static string GetRandomWords()
    {
        var random = new Random();
        var adjective = Adjectives[random.Next(Adjectives.Length)];
        var term = Terms[random.Next(Terms.Length)];
        return $"{adjective} {term}";
    }
}