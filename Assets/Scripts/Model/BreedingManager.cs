using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class BreedingManager
    {
        private List<Prey> FemalePreyBreedList;
        private List<Prey> MalePreyBreedList;

        private List<Predator> FemalePredatorBreedList;
        private List<Predator> MalePredatorBreedList;

        public BreedingManager() 
        {
            FemalePreyBreedList = new List<Prey>();
            MalePreyBreedList = new List<Prey>();
        }

        public void addToBreedList(Predator predatorToAdd) 
        {
            if (predatorToAdd.AnimalSex == Gender.Female)
            {
                if (!FemalePredatorBreedList.Contains(predatorToAdd))
                {
                    FemalePredatorBreedList.Add(predatorToAdd);
                }
            }

            else
            {
                if (!MalePredatorBreedList.Contains(predatorToAdd))
                {
                    MalePredatorBreedList.Add(predatorToAdd);
                }
            }
        }

        public void addToBreedList(Prey preyToAdd) 
        {
            
            if (preyToAdd.AnimalSex == Gender.Female)
            {
                if (!FemalePreyBreedList.Contains(preyToAdd))
                {
                    FemalePreyBreedList.Add(preyToAdd);
                }
            }

            else 
            {
                if (!MalePreyBreedList.Contains(preyToAdd))
                {
                    MalePreyBreedList.Add(preyToAdd);
                }
            }
        }

        public void removeFromBreedList(Prey preyRemove)
        {
            if (preyRemove.AnimalSex == Gender.Female)
            {
                FemalePreyBreedList.Remove(preyRemove);
            }

            else
            {
                MalePreyBreedList.Remove(preyRemove);
            }
        }

        public void removeFromBreedList(Predator predatorToRemove)
        {
            if (predatorToRemove.AnimalSex == Gender.Female)
            {
                FemalePredatorBreedList.Remove(predatorToRemove);
            }

            else
            {
                MalePredatorBreedList.Remove(predatorToRemove);
            }
        }

        public Prey FindPartner(Prey searcher) 
        {
            List<Prey> potentialMates = (searcher.AnimalSex == Gender.Male) ? FemalePreyBreedList :  MalePreyBreedList;
            Prey partner = null;

            foreach(Prey mate in potentialMates) 
            {
                Tile CurrentTile = searcher.CurrentTile;
                Tile mateLocation = mate.CurrentTile;
                partner = mate;

            }

            return partner;
        }

        public Predator FindPartner(Predator searcher)
        {
            List<Predator> potentialMates = (searcher.AnimalSex == Gender.Male) ? FemalePredatorBreedList : MalePredatorBreedList;
            Predator partner = null;

            foreach (Predator mate in potentialMates)
            {
                Tile CurrentTile = searcher.CurrentTile;
                Tile mateLocation = mate.CurrentTile;
                partner = mate;

            }

            return partner;
        }

        public void Breed(Prey searcher, Prey partner) 
        {
            Prey mother = null;
            Prey father = null;

            if (searcher.AnimalSex == Gender.Female)
            {
                mother = searcher;
                father = partner;
            }

            else
            {
                mother = partner;
                father = searcher;
            }

            removeFromBreedList(father);
            removeFromBreedList(mother);

            mother.Impregnate();
            Debug.Log(father + " Impregnated " + mother);
        }

        public void Breed(Predator searcher, Predator partner)
        {
            Predator mother = null;
            Predator father = null;

            if (searcher.AnimalSex == Gender.Female)
            {
                mother = searcher;
                father = partner;
            }

            else
            {
                mother = partner;
                father = searcher;
            }

            removeFromBreedList(father);
            removeFromBreedList(mother);

            mother.Impregnate();
            Debug.Log(father + " Impregnated " + mother);
        }
    }
}
