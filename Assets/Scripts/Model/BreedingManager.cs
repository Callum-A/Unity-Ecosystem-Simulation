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

        public BreedingManager() 
        {
            FemalePreyBreedList = new List<Prey>();
            MalePreyBreedList = new List<Prey>();
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

        public bool TryMate(Prey searcher) 
        {
            Prey partner = FindPartner(searcher);
            
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

            if (partner != null)
            {
                Breed(father, mother);
                return true;
            }

            return false;
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

        public void Breed(Prey father, Prey mother) 
        {
            removeFromBreedList(father);
            removeFromBreedList(mother);
            mother.Impregnate();

            Debug.Log(father + " Impregnated " + mother);
        }
    }
}
