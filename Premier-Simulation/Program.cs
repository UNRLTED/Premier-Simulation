using Accord;
using Accord.Statistics.Distributions.Univariate;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Premier_Simulation
{
    class Program
    {
        private static readonly double _homegoalsavg = 0.758;
        private static readonly double _awaygoalsavg = 0.603;
        private static readonly int _simulations = 1000000;

        public static void Main(string[] args)
        {
            #region Club Data
            List<Club> premierleague = new List<Club>
            {
                // Fill league with clubs
                new Club("Arsenal", 1.250, 1.048, 0.873, 0.833),
                new Club("Aston Villa", 0.764, 1.310, 0.830, 1.285),
                new Club("Bournemouth", 0.764, 1.310, 0.786, 1.215),
                new Club("Brighton and Hove Albion", 0.694, 1.179, 0.830, 0.938),
                new Club("Burnley", 0.833, 1.004, 0.830, 0.938),
                new Club("Chelsea", 1.042, 0.699, 1.703, 1.319),
                new Club("Crystal Palace", 0.521, 0.873, 0.699, 1.042),
                new Club("Everton", 0.833, 0.917, 0.873, 1.215),
                new Club("Leicester City", 1.215, 0.742, 1.397, 0.833),
                new Club("Liverpool", 1.806, 0.699, 1.441, 0.590),
                new Club("Manchester City", 1.979, 0.568, 1.965, 0.764),
                new Club("Manchester United", 1.389, 0.742, 1.135, 0.660),
                new Club("Newcastle United", 0.694, 0.917, 0.786, 1.285),
                new Club("Norwich City", 0.660, 1.616, 0.306, 1.319),
                new Club("Sheffield United", 0.833, 0.655, 0.655, 0.833),
                new Club("Southampton", 0.729, 1.528, 1.310, 0.868),
                new Club("Tottenham Hotspur", 1.250, 0.742, 1.092, 1.042),
                new Club("Watford", 0.764, 1.179, 0.611, 1.285),
                new Club("West Ham United", 1.042, 1.441, 0.830, 1.007),
                new Club("Wolverhampton Wolves", 0.938, 0.830, 1.048, 0.729)
            };
            #endregion

            #region Season Fixtures
            // Loop through each club in league to play both their home and away fixtures - Total 38 games
            for (int k = 0; k < _simulations; k++)
            {
                for (int i = 0; i < premierleague.Count; i++)
                {
                    for (int j = 0; j < premierleague.Count; j++)
                    {
                        if (i == j)
                        {
                            continue;
                        }

                        PlayFixture(premierleague[i], premierleague[j]);
                    }
                }
            }
            // Sort league by total points
            premierleague.Sort((y, x) => x.Points.CompareTo(y.Points));
            #endregion

            #region Display End of Season Table
            // Display table headers
            Console.WriteLine("Total Simulations Performed: " + _simulations.ToString("N0", CultureInfo.CreateSpecificCulture("en-US")));
            Console.WriteLine();
            Console.WriteLine($"{"Pos",-6}{"Club",-20}{"GP",13}{"W",4}{"D",4}{"L",4}{"GF",4}{"GA",4}{"GD",5}{"PTS",5}");

            for (int i = 0; i < premierleague.Count; i++)
            {
                Console.Write($"{(i + 1) + "|",4}");
                switch (i)
                {
                    case 0:
                        Console.Write($"{"C",-2}");
                        break;
                    case 1:
                    case 2:
                    case 3:
                        Console.Write($"{"L",-2}");
                        break;
                    case 4:
                    case 5:
                        Console.Write($"{"E",-2}");
                        break;
                    case 17:
                    case 18:
                    case 19:
                        Console.Write($"{"R",-2}");
                        break;
                    default:
                        Console.Write("  ");
                        break;
                }
                Console.WriteLine($"{premierleague[i].Name,-30}{(premierleague[i].Wins + premierleague[i].Losses + premierleague[i].Draws) / _simulations,3}{premierleague[i].Wins / _simulations,4}" +
                    $"{premierleague[i].Draws / _simulations,4}{premierleague[i].Losses / _simulations,4}{premierleague[i].GoalsFor / _simulations,4}{premierleague[i].GoalsAgainst / _simulations,4}" +
                    $"{(((premierleague[i].GoalsFor - premierleague[i].GoalsAgainst) / _simulations) > 0 ? "+" + ((premierleague[i].GoalsFor - premierleague[i].GoalsAgainst) / _simulations) : ((premierleague[i].GoalsFor - premierleague[i].GoalsAgainst) / _simulations).ToString()),5}{premierleague[i].Points / _simulations,5}");
            }
            Console.WriteLine();
            
            Console.WriteLine("Positions 1 - 4 Qualification for Champions League");
            Console.WriteLine("Positions 5 - 6 Qualification for Europa League");
            Console.WriteLine("Positions 18 - 20 Relegation to EFL Championship");
            #endregion
            Console.ReadLine();
        }

        // Class to hold all needed statistics about a given club
        public class Club
        {
            public string Name { get; set; }
            public int Points { get; set; }
            public int Wins { get; set; }
            public int Losses { get; set; }
            public int Draws { get; set; }
            public int GoalsFor { get; set; }
            public int GoalsAgainst { get; set; }
            public double HomeAttackStrength { get; set; }
            public double HomeDefenseStrength { get; set; }
            public double AwayAttackStrength { get; set; }
            public double AwayDefenseStrength { get; set; }

            // Constructor
            public Club(string name, double homeAttackStrength, double homeDefenseStrength, double awayAttackStrength, double awayDefenseStrength)
            {
                Name = name;
                HomeAttackStrength = homeAttackStrength;
                HomeDefenseStrength = homeDefenseStrength;
                AwayAttackStrength = awayAttackStrength;
                AwayDefenseStrength = awayDefenseStrength;

                Wins = 0;
                Losses = 0;
                GoalsFor = 0;
                GoalsAgainst = 0;
            }
        }

        public static void PlayFixture(Club home, Club away)
        {
            // Get lambda value for both teams
            double homeLambda = home.HomeAttackStrength * away.AwayDefenseStrength * _homegoalsavg;
            double awayLambda = away.AwayAttackStrength * home.HomeDefenseStrength * _awaygoalsavg;

            var poissonHome = new PoissonDistribution(homeLambda);
            var poissonAway = new PoissonDistribution(awayLambda);

            // Determine number of goals each team scored
            int homeGoals = poissonHome.Generate();
            int awayGoals = poissonAway.Generate();

            // Record game results 
            home.GoalsFor += homeGoals;
            away.GoalsFor += awayGoals;
            home.GoalsAgainst += awayGoals;
            away.GoalsAgainst += homeGoals;

            if (homeGoals > awayGoals)
            {
                home.Wins++;
                home.Points += 3;
                away.Losses++;
            }
            else if (awayGoals > homeGoals)
            {
                home.Losses++;
                away.Wins++;
                away.Points += 3;
            }
            else
            {
                home.Draws++;
                away.Draws++;
                home.Points++;
                away.Points++;
            }

        }
    }
}
