class UnitType {
    static Types: string[] = [
        'Count',
        'VolumeTeaspoons',
        'VolumeTablespoons',
        'VolumeOunces',
        'VolumeCups',
        'VolumeQuarts',
        'VolumePints',
        'VolumeGallons',
        'WeightOunces',
        'WeightPounds'
    ];
    
    static ToDisplay(str: string): string {
        if (str.startsWith('Volume')) {
            return str.substring('Volume'.length, str.length);
        }
        if (str.startsWith('Weight')) {
            let result = str.substring('Weight'.length, str.length);
            if (result === 'Ounces') {
                return 'Ounces (#)'
            }
            return result;
        }
        return str;
    }
}

export default UnitType;