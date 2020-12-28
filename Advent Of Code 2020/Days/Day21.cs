using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_Of_Code_2020.Days
{
    internal static class Day21
    {
        public static void Run()
        {
            var input = Utility.Utility.GetDayFile(21);

            var parser = new Regex("^(?:(?<ingredient>[a-z]+) )+\\(contains (?:(?<allergen>[a-z]+)(?:, )*)+\\)$");

            var ingredients = new Dictionary<string, HashSet<string>>();
            var allergens = new Dictionary<string, HashSet<string>>();
            var ingredientCounts = new Dictionary<string, int>();

            foreach (var line in input)
            {
                var match = parser.Match(line);

                var itemIngredients = match.Groups["ingredient"].Captures.Select(c => c.Value).ToList();
                var itemAllergens = match.Groups["allergen"].Captures.Select(c => c.Value).ToList();

                foreach (var ingredient in itemIngredients)
                {
                    if (!ingredients.TryGetValue(ingredient, out var potentialAllergens))
                    {
                        potentialAllergens = new HashSet<string>(itemAllergens);
                        ingredients[ingredient] = potentialAllergens;
                        ingredientCounts[ingredient] = 1;
                    }
                    else
                    {
                        potentialAllergens.UnionWith(itemAllergens);
                        ingredientCounts[ingredient]++;
                    }
                }

                foreach (var allergen in itemAllergens)
                {
                    if (!allergens.TryGetValue(allergen, out var potentialIngredients))
                    {
                        potentialIngredients = new HashSet<string>(itemIngredients);
                        allergens[allergen] = potentialIngredients;
                    }
                    else
                    {
                        potentialIngredients.IntersectWith(itemIngredients);
                    }
                }
            }

            var safeIngredients = ingredients.Keys.Except(allergens.Values.SelectMany(s => s));

            Console.WriteLine($"Time safe ingredients appear: {safeIngredients.Sum(ingredient => ingredientCounts[ingredient])}");

            foreach (var safeIngredient in safeIngredients)
            {
                ingredients.Remove(safeIngredient);
            }

            foreach (var allergen in allergens)
            {
                allergen.Value.ExceptWith(safeIngredients);
            }

            var matched = new Dictionary<string, string>();

            while (allergens.Any())
            {
                var progress = false;

                var foundAllergen = allergens.FirstOrDefault(allergen => allergen.Value.Count == 1);
                if (foundAllergen.Key != null)
                {
                    var foundAllergenIngredient = foundAllergen.Value.Single();
                    matched.Add(foundAllergen.Key, foundAllergenIngredient);

                    allergens.Remove(foundAllergen.Key);
                    ingredients.Remove(foundAllergenIngredient);

                    foreach(var allergenIngredient in allergens.Values.Where(ai => ai.Contains(foundAllergenIngredient)))
                    {
                        allergenIngredient.Remove(foundAllergenIngredient);
                    }

                    foreach (var ingredientAllergens in ingredients.Values.Where(ia => ia.Contains(foundAllergen.Key)))
                    {
                        ingredientAllergens.Remove(foundAllergen.Key);
                    }

                    progress = true;
                }

                var foundIngredient = ingredients.FirstOrDefault(ingredient => ingredient.Value.Count == 1);
                if (foundIngredient.Key != null)
                {
                    var foundIngredientAllergen = foundIngredient.Value.Single();
                    matched.Add(foundIngredientAllergen, foundIngredient.Key);

                    allergens.Remove(foundIngredientAllergen);
                    ingredients.Remove(foundIngredient.Key);

                    foreach(var allergenIngredients in allergens.Values.Where(ai => ai.Contains(foundIngredient.Key)))
                    {
                        allergenIngredients.Remove(foundIngredient.Key);
                    }

                    foreach (var ingredientAllergens in ingredients.Values.Where(ia => ia.Contains(foundIngredientAllergen)))
                    {
                        ingredientAllergens.Remove(foundIngredientAllergen);
                    }

                    progress = true;
                }

                if (!progress) throw new InvalidOperationException("No progress made");
            }

            Console.WriteLine(string.Join(",", matched.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value)));
        }
    }
}