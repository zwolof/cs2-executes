
using System.Runtime.Serialization;

namespace CS2Executes.Enums
{
	public enum EBombsite
	{
		A = 0,

		B = 1
	}

	public static class EBombsiteExtension
	{
		public static string ToString(this EBombsite bombsite) 
		{
			return bombsite switch
			{
				EBombsite.A => "A",
				EBombsite.B => "B",
				_ => "None",
			};
		}

		public static EBombsite ToEnum(this string bombsite)
		{
			return bombsite switch
			{
				"A" => EBombsite.A,
				"B" => EBombsite.B,
				_ => EBombsite.A,
			};
		}
	}
}
