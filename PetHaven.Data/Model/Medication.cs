﻿namespace PetHaven.Data.Model
{
    public class Medication : BaseEntity
    {
        public string Name { get; set; }

        public string Dosage { get; set; }

        public string Frequency { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Notes { get; set; }

        public int PetId { get; set; }
        public virtual Pet? Pet { get; set; }

    }
}
