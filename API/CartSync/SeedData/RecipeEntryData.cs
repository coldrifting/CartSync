using CartSync.Data.Entities;
using CartSync.Objects;

namespace CartSync.SeedData;

// ReSharper disable StringLiteralTypo
public static partial class SeedData
{
    public static List<RecipeEntry> RecipeEntries =>
    [
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH3FCHDZPZ39KAPGF00"), RecipeSectionId = RecipeSections[0].RecipeSectionId, ItemId = Items[44].ItemId, PrepId = null, Amount =             Amount.WeightOunces(12) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH31P7T4D0TB91FAVC1"), RecipeSectionId = RecipeSections[0].RecipeSectionId, ItemId = Items[193].ItemId, PrepId = null, Amount =            Amount.VolumeCups(4) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH363FVT6EYFPGH3XTN"), RecipeSectionId = RecipeSections[0].RecipeSectionId, ItemId = Items[149].ItemId, PrepId = null, Amount =            Amount.VolumeTablespoons(2) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH3T0R1GTVNHRMAB080"), RecipeSectionId = RecipeSections[0].RecipeSectionId, ItemId = Items[207].ItemId, PrepId = Preps[2].PrepId, Amount = Amount.Count(2) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH3BMNZXNCEVY82F81X"), RecipeSectionId = RecipeSections[0].RecipeSectionId, ItemId = Items[147].ItemId, PrepId = null, Amount =            Amount.VolumeCups(2) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH34647NBD3HSXET0PT"), RecipeSectionId = RecipeSections[0].RecipeSectionId, ItemId = Items[177].ItemId, PrepId = null, Amount =            Amount.VolumeOunces(6) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH395D5QJPB29VQ4XHK"), RecipeSectionId = RecipeSections[0].RecipeSectionId, ItemId = Items[176].ItemId, PrepId = null, Amount =            Amount.VolumeCups(3,4) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4BSQAHQF5MJ1AYF2P"), RecipeSectionId = RecipeSections[0].RecipeSectionId, ItemId = Items[88].ItemId, PrepId = null, Amount =             Amount.VolumeTeaspoons(1,4) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4RWHJZZH6F6MQPQFK"), RecipeSectionId = RecipeSections[0].RecipeSectionId, ItemId = Items[66].ItemId, PrepId = null, Amount =             Amount.VolumeTeaspoons(1,8) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4AXKZZ555BB0954R4"), RecipeSectionId = RecipeSections[1].RecipeSectionId, ItemId = Items[46].ItemId, PrepId = null, Amount =             Amount.WeightOunces(8) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH40XJ9XY1WYN1PV0AM"), RecipeSectionId = RecipeSections[1].RecipeSectionId, ItemId = Items[149].ItemId, PrepId = null, Amount =            Amount.VolumeTablespoons(2) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4FTJFDNY07YD99422"), RecipeSectionId = RecipeSections[1].RecipeSectionId, ItemId = Items[100].ItemId, PrepId = null, Amount =            Amount.VolumeTablespoons(2) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4T6NMT7FSN8EBFEFH"), RecipeSectionId = RecipeSections[1].RecipeSectionId, ItemId = Items[147].ItemId, PrepId = null, Amount =            Amount.VolumeCups(2) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4FJXQ2DHCTAZ1B4HD"), RecipeSectionId = RecipeSections[1].RecipeSectionId, ItemId = Items[180].ItemId, PrepId = Preps[3].PrepId, Amount = Amount.VolumeOunces(8) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4523TNY0EG9NZF22Q"), RecipeSectionId = RecipeSections[1].RecipeSectionId, ItemId = Items[63].ItemId, PrepId = null, Amount =             Amount.VolumeOunces(8) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH40EYAXZMGNXP7EEH9"), RecipeSectionId = RecipeSections[1].RecipeSectionId, ItemId = Items[74].ItemId, PrepId = null, Amount =             Amount.VolumeTeaspoons(1,2) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4K2SQ2NEG1A70QRDX"), RecipeSectionId = RecipeSections[1].RecipeSectionId, ItemId = Items[73].ItemId, PrepId = null, Amount =             Amount.VolumeTeaspoons(1,8) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4W77771AK78T6KQWV"), RecipeSectionId = RecipeSections[1].RecipeSectionId, ItemId = Items[88].ItemId, PrepId = null, Amount =             Amount.VolumeTeaspoons(1,4) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4CDARF5EQ3YQR0K7P"), RecipeSectionId = RecipeSections[2].RecipeSectionId, ItemId = Items[205].ItemId, PrepId = Preps[4].PrepId, Amount = Amount.Count(1,2) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4EVBQDSHE2RAHV2D5"), RecipeSectionId = RecipeSections[2].RecipeSectionId, ItemId = Items[209].ItemId, PrepId = Preps[5].PrepId, Amount = Amount.VolumeCups(1) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH40ZTPNJYFDG6HZE9R"), RecipeSectionId = RecipeSections[2].RecipeSectionId, ItemId = Items[217].ItemId, PrepId = null, Amount =            Amount.Count(1,4) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH497MAWKY1420ZXV3B"), RecipeSectionId = RecipeSections[2].RecipeSectionId, ItemId = Items[23].ItemId, PrepId = null, Amount =             Amount.Count(8) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4C68KGN3A5SS6TF9P"), RecipeSectionId = RecipeSections[2].RecipeSectionId, ItemId = Items[178].ItemId, PrepId = Preps[0].PrepId, Amount = Amount.VolumeOunces(2) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4WD0NKQ3ET9JCWQEV"), RecipeSectionId = RecipeSections[2].RecipeSectionId, ItemId = Items[175].ItemId, PrepId = null, Amount =            Amount.Count(4) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4MWRPCW0EW7AHAP8C"), RecipeSectionId = RecipeSections[2].RecipeSectionId, ItemId = Items[187].ItemId, PrepId = null, Amount =            Amount.VolumeCups(1) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4R00YMH9MXFWGBRFD"), RecipeSectionId = RecipeSections[2].RecipeSectionId, ItemId = Items[5].ItemId, PrepId = null, Amount =              Amount.VolumeTeaspoons(2) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4X21E6VEQHD5PYQAZ"), RecipeSectionId = RecipeSections[2].RecipeSectionId, ItemId = Items[88].ItemId, PrepId = null, Amount =             Amount.VolumeTeaspoons(1,8) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4P7YMV83GW7QVERCZ"), RecipeSectionId = RecipeSections[2].RecipeSectionId, ItemId = Items[66].ItemId, PrepId = null, Amount =             Amount.VolumeTeaspoons(1,8) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH444HBCDRS4TGBBBEA"), RecipeSectionId = RecipeSections[2].RecipeSectionId, ItemId = Items[180].ItemId, PrepId = null, Amount =            Amount.VolumeCups(1) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4KY2YP63QXWW827CW"), RecipeSectionId = RecipeSections[3].RecipeSectionId, ItemId = Items[215].ItemId, PrepId = Preps[5].PrepId, Amount = Amount.Count(4) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4FVNQ2MKDG1NRT7A6"), RecipeSectionId = RecipeSections[3].RecipeSectionId, ItemId = Items[231].ItemId, PrepId = null, Amount =            Amount.VolumeTablespoons(2) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4WCWPDTE9MRZW46E3"), RecipeSectionId = RecipeSections[3].RecipeSectionId, ItemId = Items[217].ItemId, PrepId = Preps[1].PrepId, Amount = Amount.Count(1,3) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH482PV5FHWQX72KJT3"), RecipeSectionId = RecipeSections[3].RecipeSectionId, ItemId = Items[216].ItemId, PrepId = Preps[4].PrepId, Amount = Amount.Count(1,2) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4ZYX439B3SG7XNQX2"), RecipeSectionId = RecipeSections[3].RecipeSectionId, ItemId = Items[74].ItemId, PrepId = null, Amount =             Amount.VolumeTeaspoons(1,2) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH42297N6GTHJ3BGKR8"), RecipeSectionId = RecipeSections[3].RecipeSectionId, ItemId = Items[72].ItemId, PrepId = null, Amount =             Amount.VolumeTeaspoons(1,2) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4W4RM6D78X32YW8HM"), RecipeSectionId = RecipeSections[3].RecipeSectionId, ItemId = Items[85].ItemId, PrepId = null, Amount =             Amount.VolumeTeaspoons(1,2) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4Y8CF88Y6CY256KN2"), RecipeSectionId = RecipeSections[3].RecipeSectionId, ItemId = Items[207].ItemId, PrepId = Preps[2].PrepId, Amount = Amount.Count(1) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4ZCJ5X56C04A860WC"), RecipeSectionId = RecipeSections[3].RecipeSectionId, ItemId = Items[30].ItemId, PrepId = null, Amount =             Amount.VolumeOunces(8) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4RXCBEK6A5JDP58RR"), RecipeSectionId = RecipeSections[3].RecipeSectionId, ItemId = Items[56].ItemId, PrepId = null, Amount =             Amount.VolumeCups(1) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4CNQ4ZRF0BW1WZWQN"), RecipeSectionId = RecipeSections[3].RecipeSectionId, ItemId = Items[193].ItemId, PrepId = null, Amount =            Amount.VolumeCups(3) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4SA22ZN23BS443VRR"), RecipeSectionId = RecipeSections[3].RecipeSectionId, ItemId = Items[61].ItemId, PrepId = null, Amount =             Amount.VolumeTablespoons(2) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4NABG3FMVFEH2PQH4"), RecipeSectionId = RecipeSections[3].RecipeSectionId, ItemId = Items[64].ItemId, PrepId = null, Amount =             Amount.VolumeCups(3,4) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4F2A0BJ08FMW25C65"), RecipeSectionId = RecipeSections[3].RecipeSectionId, ItemId = Items[88].ItemId, PrepId = null, Amount =             Amount.VolumeTeaspoons(1,4) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4GD4Y800F7030CVM1"), RecipeSectionId = RecipeSections[4].RecipeSectionId, ItemId = Items[66].ItemId, PrepId = null, Amount =             Amount.VolumeTeaspoons(1,4) },
        new() { RecipeEntryId = Ulid.Parse("01KJA5YNH4VDCN7FMVAP9BRXT7"), RecipeSectionId = RecipeSections[4].RecipeSectionId, ItemId = Items[180].ItemId, PrepId = Preps[3].PrepId, Amount = Amount.VolumeCups(1) }
    ];
}