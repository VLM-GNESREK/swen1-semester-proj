// Program.cs

using Treasure_Bay.Classes;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        TypeFast("###------------------------------###");
        PromptSlowly("#   Welcome to the Treasure Bay!   #");
        TypeFast("###------------------------------###");

        // User Registration (placeholder)

        PrintSlowly("\nAhoy Matey, be ye new to these parts eh?");
        PrintSlowly("What's yer name?");
        System.Console.Write("\n\n");
        PromptSlowlyEx(">Enter Username: ");
        string username = Console.ReadLine();
        User user1 = new User(username, 1, "hashed_password_trust_tm");
        PrintSlowly($"\n\nThat so {user1.Username}?");
        PrintSlowly("Well welcome aboard!");
        PrintSlowly("Ah, but what have ye there?");
        PrintSlowly("Why yes! It must be a Treasure!");
        PrintSlowly("What's its name?\n");

        // Media Registration (placeholder)

        PromptSlowlyEx(">Enter Media Title: ");
        string mediaTitle = Console.ReadLine();
        MediaEntry media = new MediaEntry(42, mediaTitle, "An awesome treasure.", 2025, user1);
        PrintSlowly($"\n\n{media.Title}, a truly fine name for such a specimen!");
        PrintSlowly($"Tell me more about this {media.Title} of yours!\n");
        PromptSlowlyEx(">Description: ");
        string description = Console.ReadLine();
        media.Description = description;

        // Rating stuffu

        PrintSlowly("\n\nA fine tale! But what's yer final word on it?");
        PrintSlowly("How many doubloons... eh... 'stars' be it worth? (1-5)");
        PromptSlowlyEx("\n>Enter Rating (1-5): ");
        string starString = Console.ReadLine();
        int stars = int.Parse(starString);

        // Comment

        PrintSlowly("\n\nAnd any last words for the ship's log?");
        PromptSlowlyEx("\n>Enter Comment: ");
        string comment = Console.ReadLine();
        user1.PostRating(media, stars, comment); // Post rating
        PrintSlowly("\nSplendid! We've marked it in the ledger.");
        PrintSlowly("Let's check the scrolls to be sure...");

        // Verification

        TypeFast("\n--- Ledger Verification ---");
        PrintSlowly($"Captain '{user1.Username}' has {user1.Ratings.Count} entry in their logbook.");
        PrintSlowly($"The treasure '{media.Title}' has been rated {media.Ratings.Count} time(s).");

        // Deets
        PrintSlowly($"There are {media.Ratings.Count} Ratings");
        foreach (Rating rating in media.Ratings)
        {
            PrintSlowly($"\nThe first entry reads: {rating.StarValue} stars.");
            PrintSlowly($"The log says: '{rating.Comment}'");
            PrintSlowly($"Signed by: {rating.Reviewer.Username}");
        }
        // See yourself out

         PrintSlowly("\n\nOur business be done here. Press Enter to set sail.");
         Console.ReadLine();
    }

    // Print is too fast aahhh methods

    static void PrintSlowly(string text, int delay = 40)
    {
        foreach (char c in text)
        {
            Console.Write(c);
            Thread.Sleep(delay); // sleeps for 40ms
        }
        Console.WriteLine(); // ol'reliable \n
    }

    static void PromptSlowly(string text, int delay = 20)
    {
        foreach (char c in text)
        {
            Console.Write(c);
            Thread.Sleep(delay); // sleeps for 70ms
        }
        Console.WriteLine(); // it hasn't changed, you know?
    }

    static void TypeFast(string text, int delay = 5)
    {
        foreach (char c in text)
        {
            Console.Write(c);
            Thread.Sleep(delay); // sleeps for 5ms
        }
        Console.WriteLine(); // still here?
    }

    static void PromptSlowlyEx(string text, int delay = 20)
    {
        foreach (char c in text)
        {
            Console.Write(c);
            Thread.Sleep(delay); // sleeps for 20ms
        }
        // There, I removed it. Happy now?
    }
}