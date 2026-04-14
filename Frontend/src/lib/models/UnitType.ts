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
    
    static asString(str: string): string {
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
    
    static asAbbreviation(str: string): string {
        switch (str) {
            case 'Count':
                return 'ea.'
            case 'VolumeTeaspoons':
                return 'tsp.'
            case 'VolumeTablespoons':
                return 'Tbsp.'
            case 'VolumeOunces':
                return 'oz.'
            case 'VolumeCups':
                return 'cups'
            case 'VolumeQuarts':
                return 'qt.'
            case 'VolumePints':
                return 'pt.'
            case 'VolumeGallons':
                return 'gal.'
            case 'WeightOunces':
                return 'oz.'
            case 'WeightPounds':
                return 'lbs'
            default:
                return str;
        }
    }
}

export default UnitType;