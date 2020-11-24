using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreCodeCamp.Models
{
	public class CampModel
	{
		[Required]
		[StringLength(100)]
		public string Name { get; set; }
		[Required]
		public string Moniker { get; set; }
		public string Venue { get; set; }
		public DateTime EventDate { get; set; } = DateTime.MinValue;
		[Range(1,100)]
		public int Length { get; set; } = 1;
		public ICollection<TalkModel> Talks { get; set; }
	}
}