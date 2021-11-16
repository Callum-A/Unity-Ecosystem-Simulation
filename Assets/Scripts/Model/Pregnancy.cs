﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Model
{
    public class Pregnancy
    {
        private Prey mother;
        public float TimeUntilBirth { get; protected set; }
        

        public Pregnancy(Prey mother) 
        {
            TimeUntilBirth = TimeController.Instance.SECONDS_IN_A_DAY * 3;
            this.mother = mother;
        }

        public void UpdatePregnancy(float deltatime) 
        {
            TimeUntilBirth -= deltatime;

            if (TimeUntilBirth <= 0) 
            {
                GiveBirth();
            }
        }

        private void GiveBirth() 
        {
            mother.GiveBirth();
        }

        private void MisCarry() { }
    }
}