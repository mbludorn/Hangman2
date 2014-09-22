using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hangman
{
    class Hangman
    {
        //Below are the member variables for the class Hangman

        public static int noOfLives = 7;
        public static List<string> wordBank = new List<string>() 
        {"apple", "nickeback", "computer", "seedpaths", 
        "mountain", "sky", "river", "chair", "boy", "afternoon", "watermelon", "television", "bus", "water", "glass"};
        public static string randomWord;
        public static Random rng = new Random();
        public static string lettersGuessed = string.Empty;
        public static string usersGuess;
        public static bool won = false;
        public static string userName;
        public static int score = 0;

        static void Main(string[] args)
        {
            //generated a random word
            randomWord = wordBank[rng.Next(1, 16)];

            //Geeting the user and explaing the rules by calling below function
            Greet();

            //Playing the game. 
            //Divided each task into it's own function, so that code will be easy to manage and main looks clean
            PlayGame();

            AddScoreToDB();
            DisplayFromDB();
        }

        //function to greet the user and display rules
        static void Greet()
        {
            Console.WriteLine("Enter your name");
            userName = Console.ReadLine();

            Console.WriteLine("\nWelcome to Hangman game: " + userName);
            Console.WriteLine("\nHandman Rules:\nYou need to guess the whole word or a single letter in the word");
            Console.WriteLine("You will win, if you guess the whole word or all the letters in the word");
            Console.WriteLine("You only have " + noOfLives + " lives to guess");
            Console.WriteLine("If you can't guess in " + noOfLives + " lives, you will lose");
            Console.WriteLine("Good luck!");
        }


        //function to display Masked word
        static int MaskedWord(string randomWord)
        {
            int count = 0;
            for (int i = 0; i < randomWord.Length; i++)
            {
                if (lettersGuessed.Contains(randomWord[i]))
                {
                    Console.Write(randomWord[i]);
                }
                else
                {
                    Console.Write(" _ ");
                    count++;
                }
            }
            return count;

        }

        //function contains the logic for the game
        static void PlayGame()
        {
            int count;

            while (won != true)
            {

                count = MaskedWord(randomWord);
                if (count == 0)
                {
                    Console.WriteLine("\nCongratulations, " + userName + " you won!");
                    Console.WriteLine("The word is: " + randomWord);
                    won = true;
                }

                else
                {
                    //get user's guess
                    Console.WriteLine("\n\nYou have " + noOfLives + " lives left. Guess");
                    usersGuess = Console.ReadLine();
                    UsersGuessVerify(usersGuess);
                    if ((noOfLives == 0) && (won == false))
                    {
                        Console.WriteLine("\nSorry " + userName + " you ran out of lives. Game over");
                        won = true;
                        score = 0;
                    }
                }

            }

        }

        //function to verify users guess
        static void UsersGuessVerify(string guess)
        {
            //verify if user entered a letter or word
            if (guess.Length > 1) //means user entered a word
            {
                if (guess == randomWord)
                {
                    Console.WriteLine("Congratulations, " + userName + " you won!");
                    //Console.WriteLine(randomWord);
                    won = true;
                    noOfLives = 0;
                    score += 100;
                    return;
                }
                else
                {
                    noOfLives--;
                    Console.WriteLine("You guessed wrong. You have " + noOfLives + " lives left");
                    Console.WriteLine("You have guessed: " + lettersGuessed);
                    score--;
                    return;
                }
            }
            else //he entered a letter
            {
                //add that letter to the guessed letters
                lettersGuessed = lettersGuessed + guess;

                //verify if the word to be guessed has the letter entered by user
                if (randomWord.Contains(guess))
                {
                    score += 10;
                    return;
                }
                else
                {
                    noOfLives--;
                    Console.WriteLine("You guessed wrong. You have " + noOfLives + " lives left");
                    Console.WriteLine("You have guessed: " + lettersGuessed);
                    score--;
                    return;
                }

            }
        }

        //add score to data base
        static void AddScoreToDB()
        {
            Hangman2.JayaEntities db = new Hangman2.JayaEntities();

            Hangman2.HighScore currentScore = new Hangman2.HighScore();

            currentScore.DateCreated = DateTime.Now;
            currentScore.Game = "Hangman";
            currentScore.Name = userName;
            currentScore.Score = score;

            db.HighScores.Add(currentScore);
            db.SaveChanges();
        }

        //display data from the data base
        static void DisplayFromDB()
        {
            Hangman2.JayaEntities db = new Hangman2.JayaEntities();

            Console.WriteLine("Press Enter to see high scores");
            Console.ReadKey();
            Console.Clear();

            Console.WriteLine("High scores of Hangman");
            Console.WriteLine("=======================");

            List<Hangman2.HighScore> highScoresList = db.HighScores.Where(x => x.Game == "Hangman").OrderByDescending(x => x.Score).Take(10).ToList();

            foreach (var item in highScoresList)
            {
                Console.WriteLine("{0}, {1}{2}  {3}", highScoresList.IndexOf(item)+1, item.Name, item.Score, item.DateCreated);
            }
        }


    }
}
