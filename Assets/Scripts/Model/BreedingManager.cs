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
            FemalePredatorBreedList = new List<Predator>();
            MalePredatorBreedList = new List<Predator>();

        }

        public void addToBreedList(Animal a) 
        {
            if (a is Predator)
            {
                addToBreedList(a as Predator);
            }

            else 
            {
                addToBreedList(a as Prey);
            }
        }

        private void addToBreedList(Predator predatorToAdd) 
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

        private void addToBreedList(Prey preyToAdd) 
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

        public void removeFromBreedList(Animal a) 
        {
            if (a is Predator)
            {
                removeFromBreedList(a as Predator);
            }

            else 
            {
                removeFromBreedList(a as Prey);
            }
        }

        private void removeFromBreedList(Prey preyRemove)
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

        private void removeFromBreedList(Predator predatorToRemove)
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

        public Animal FindPartner(Animal a) 
        {
            Animal partner = null;

            if (a is Predator)
            {
                partner = FindPartner(a as Predator);
            }

            else 
            {
                partner = FindPartner(a as Prey);
            }

            if (partner != null) 
            {
                removeFromBreedList(partner);
                removeFromBreedList(a);
            }

            return partner;
        }

        private Prey FindPartner(Prey searcher) 
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

        private Predator FindPartner(Predator searcher)
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

        public void Breed(Animal searcher, Animal partner) 
        {
            if (searcher is Predator && partner is Predator)
            {
                Breed(searcher as Predator, partner as Predator);
            }

            else 
            {
                Breed(searcher as Prey, partner as Prey);
            }
        }

        private void Breed(Prey searcher, Prey partner) 
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

        private void Breed(Predator searcher, Predator partner)
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

            mother.Impregnate();
            Debug.Log(father + " Impregnated " + mother);
        }
    }
}
