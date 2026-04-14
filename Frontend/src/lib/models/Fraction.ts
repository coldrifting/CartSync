class Fraction {
    num: number = 1;
    dem: number = 1;
    
    static asString(fraction: Fraction, prefixed: boolean = false): string {
        if (fraction.dem === 1 || fraction.num === fraction.dem) {
            return fraction.num.toString();
        }
        
        if (fraction.num > fraction.dem) {
            return (fraction.num / fraction.dem | 0).toFixed(0) + Fraction.asString({num: (fraction.num % fraction.dem), dem: fraction.dem} as Fraction, true);
        }
        
        return Fraction.asSingleChar(fraction.num, fraction.dem, prefixed);
    }
    
    static asNumber(fraction: Fraction): number {
        if (fraction.dem === 1) {
            return fraction.num;
        }

        return Number.parseFloat((Math.floor((fraction.num * 1000) / fraction.dem) / 1000.0).toFixed(3));
    }
    
    static fromNumberString(fractionStr: string): Fraction {
        const backing: number = Number.parseFloat(fractionStr) * 1000;
        
        const whole: number = Number.parseInt(fractionStr) || 0;
        const frac: number = backing % 1000;
        
        if (frac === 0) {
            return {num: whole, dem: 1};
        }
        
        switch (frac) {
            case 160:
            case 166:
            case 167:
                return {num: (whole * 6) + 1, dem: 6}
            
            case 830:
            case 833:
            case 834:
                return {num: (whole * 6) + 5, dem: 6}
            
            case 330:
            case 333:
            case 334:
                return {num: (whole * 3) + 1, dem: 3}
                
            case 660:
            case 666:
            case 667:
                return {num: (whole * 3) + 2, dem: 3}
        }
        
        for (let divisor: number = 2; divisor <= 16; divisor++) {
            if ((frac * divisor) % 1000 == 0)
            {
                return {num: (whole * divisor) + ((frac * divisor) / 1000), dem: divisor}
            }
        }
        
        // Fallback
        return {num: (whole * 1000) + frac, dem: 1000};
    }
    
    static fromString(fractionStr: string): Fraction {
        let split: string[] = fractionStr.split("/", 2);
        if (split.length === 1) {
            let value: number = parseInt(split[0]);
            if (!Number.isNaN(value) && value > 0) {
                return {num: value, dem: 1} as Fraction;
            }
        }
        if (split.length === 2) {
            let num: number = parseInt(split[0]);
            let dem: number = parseInt(split[1]);
            if (!Number.isNaN(num) && !Number.isNaN(dem) && num > 0 && dem > 0) {
                return {num: num, dem: dem} as Fraction;
            }
        }
        
        return {num: 1, dem: 1} as Fraction;
    }
    
    static isValid(fractionStr: string): boolean {
        let split: string[] = fractionStr.split("/", 2);
        if (split.length === 1) {
            let value: number = parseInt(split[0]);
            if (!Number.isNaN(value) && value > 0) {
                return true;
            }
        }
        if (split.length === 2) {
            let num: number = parseInt(split[0]);
            let dem: number = parseInt(split[1]);
            if (!Number.isNaN(num) && !Number.isNaN(dem) && num > 0 && dem > 0) {
                return true;
            }
        }
        
        return false;
    }
    
    static asSingleChar(num: number, dem: number, prefixed: boolean): string {
        switch (`${num}/${dem}`)
        {
            case "1/2": return prefixed ? " ½" : "½";

            case "1/3":
            case "33/100":
            case "333/1000": return prefixed ? " ⅓" : "⅓";
            
            case "2/3":
            case "66/100":
            case "666/1000": return prefixed ? " ⅔" : "⅔";
            
            case "1/4": return prefixed ? " ¼" :  "¼";
            case "3/4": return prefixed ? " ¾" :  "¾";

            case "1/5": return prefixed ? " ⅕" :  "⅕";
            case "2/5": return prefixed ? " ⅖" :  "⅖";
            case "3/5": return prefixed ? " ⅗" :  "⅗";
            case "4/5": return prefixed ? " ⅘" :  "⅘";

            case "1/6": return prefixed ? " ⅙" :  "⅙";
            case "5/6": return prefixed ? " ⅚" :  "⅚";

            case "1/8": return prefixed ? " ⅛" :  "⅛";
            case "3/8": return prefixed ? " ⅜" :  "⅜";
            case "5/8": return prefixed ? " ⅝" :  "⅝";
            case "7/8": return prefixed ? " ⅞" :  "⅞";
            
            case "1/10": return prefixed ? " ⅒" : "⅒";
            case "3/10": return prefixed ? " ³⁄₁₀" : "³⁄₁₀";
            case "7/10": return prefixed ? " ⁷⁄₁₀" : "⁷⁄₁₀";
            case "9/10": return prefixed ? " ⁹⁄₁₀" : "⁹⁄₁₀";
            
            default:
                if (dem < 20) {
                    return prefixed ? ` ${num}/${dem}` : `${num}/${dem}`;
                }
                
                const decimalString: string = (num / dem).toFixed(3);
                if (prefixed) {
                    return decimalString.replace("0.", '.');
                }
                return decimalString;
        }
        
    }
}
    
export default Fraction;