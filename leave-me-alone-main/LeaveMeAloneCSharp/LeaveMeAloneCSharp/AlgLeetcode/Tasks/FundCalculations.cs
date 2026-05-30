using System;

public class FundCalculations
{
    // Входные параметры для расчетов
    public double FK_Total; // Общий фондовый капитал
    public double[] UFK; // Массив фондового капитала по каждому виду IA
    public double[] PFK; // Процент фондового капитала
    public double[] NAVPS; // Чистая стоимость активов на акцию
    public double ACT; // Общая сумма активов
    public double[] a; // Массив коэффициентов для IA
    public double UFK_PIA, UFK_PRIA, UFK_VIA; // Капитал для PIA, PRIA, VIA

    public FundCalculations(double fk_total, double[] ufk, double[] pfk, double[] navps, double act, double[] aValues,
                            double ufk_pia, double ufk_pria, double ufk_via)
    {
        FK_Total = fk_total;
        UFK = ufk;
        PFK = pfk;
        NAVPS = navps;
        ACT = act;
        a = aValues;
        UFK_PIA = ufk_pia;
        UFK_PRIA = ufk_pria;
        UFK_VIA = ufk_via;
    }

    public void Calculate()
    {
        // 2.3. Mezivýpočty (Промежуточные расчёты)
        double Y = FK_Total - Sum(UFK); // Пересчитанный абсолютный доход за референсный период

        double[] Y_x = new double[PFK.Length]; // Абсолютный доход для x-того IA
        for (int i = 0; i < PFK.Length; i++)
        {
            Y_x[i] = Y * PFK[i];
        }

        // Гипотетический абсолютный доход при 7% годовых
        double[] Y_x7 = new double[NAVPS.Length];
        for (int i = 0; i < NAVPS.Length; i++)
        {
            Y_x7[i] = NAVPS[i] * (7 * 0.01 / ACT) * a[i];
        }

        // Гипотетический абсолютный доход при 5% годовых
        double Y_PIA5 = NAVPS[0] * (5 * 0.01 / ACT) * a[0];

        // Гипотетический абсолютный доход при 8% годовых
        double Y_PRIA8 = NAVPS[1] * (8 * 0.01 / ACT) * a[1];

        // Минимальный доход
        double Y_min = Y_PIA5 + Y_PRIA8;
        double Y_Pmin = Y_PIA5 + Y_PRIA8 + Y_x7[2];

        // 2.4 Výpočty FK x t (Основные расчеты фондового капитала)
        double FK_PIA, FK_PRIA, FK_VIA;

        if (Y > Y_min)
        {
            FK_PIA = UFK_PIA + Y_x7[0] + Math.Min(Y_PIA5, (Y_PIA5 - Y_x7[0]) * 0.7);
            FK_PRIA = UFK_PRIA + Y_x7[1] + Math.Min(Y_PRIA8, (Y_PRIA8 - Y_x7[1]) * 0.75);
            FK_VIA = UFK_VIA + Y - Y_Pmin - Math.Min(Y_PIA5, (Y_PIA5 - Y_x7[0]) * 0.7) - Math.Min(Y_PRIA8, (Y_PRIA8 - Y_x7[1]) * 0.75);
        }
        else if (Y <= Y_min && Y > Y_Pmin)
        {
            FK_PIA = UFK_PIA + Y_x7[0];
            FK_PRIA = UFK_PRIA + Y_x7[1];
            FK_VIA = UFK_VIA + Y - Y_Pmin;
        }
        else if (Y <= Y_Pmin && Y > 0)
        {
            FK_PIA = UFK_PIA + Y * (Y_x7[0] / Y_Pmin);
            FK_PRIA = UFK_PRIA + Y * (Y_x7[1] / Y_Pmin);
            FK_VIA = UFK_VIA;
        }
        else
        {
            FK_PIA = UFK_PIA + Y_x[0];
            FK_PRIA = UFK_PRIA + Y_x[1];
            FK_VIA = UFK_VIA + Y_x[2];
        }

        // Вывод результатов
        Console.WriteLine($"FK_PIA: {FK_PIA}, FK_PRIA: {FK_PRIA}, FK_VIA: {FK_VIA}");
    }

    private double Sum(double[] array)
    {
        double sum = 0;
        foreach (double num in array)
        {
            sum += num;
        }
        return sum;
    }
}

public class FundCalculations2
{
    public double FK_Total; // Celkový fondový kapitál
    public double[] UFK; // Upravený fondový kapitál pro každý druh IA
    public double[] PFK; // Podíl fondového kapitálu
    public double[] NAVPS; // Čistá hodnota aktiv na akcii
    public double[] div; // Dividendy pro každý druh IA
    public double[] a; // Počet investičních akcií
    public double ACT; // Celkový počet dní v roce

    public FundCalculations2(double fk_total, double[] navps, double[] divs, double[] aValues, double act)
    {
        FK_Total = fk_total;
        NAVPS = navps;
        div = divs;
        a = aValues;
        ACT = act;
        UFK = new double[navps.Length];
        PFK = new double[navps.Length];
    }

    public void Calculate()
    {
        // 1. Výpočet UFK_x_r-1 (upravený fondový kapitál z předchozího referenčního období)
        for (int i = 0; i < NAVPS.Length; i++)
        {
            UFK[i] = (NAVPS[i] - div[i]) * a[i];
        }

        // 2. Výpočet PFK_x_r-1 (podíl upraveného fondového kapitálu)
        double total_UFK = Sum(UFK);
        for (int i = 0; i < UFK.Length; i++)
        {
            PFK[i] = total_UFK > 0 ? UFK[i] / total_UFK : 0;
        }

        // 3. Výpočet absolutního výnosu za období
        double Y = FK_Total - total_UFK;
        double[] Y_x = new double[PFK.Length];
        for (int i = 0; i < PFK.Length; i++)
        {
            Y_x[i] = Y * PFK[i];
        }

        // 4. Výpočet hypotetického výnosu při 7 % ročně
        double[] Y_x7 = new double[NAVPS.Length];
        for (int i = 0; i < NAVPS.Length; i++)
        {
            Y_x7[i] = NAVPS[i] * (7 * 0.01 / ACT) * a[i];
        }

        double Y_PIA7 = Y_x7[0];
        double Y_PRIA7 = Y_x7[1];
        double Y_VIA7 = Y_x7[2];

        double Y_PIA5 = NAVPS[0] * (5 * 0.01 / ACT) * a[0];
        double Y_PRIA8 = NAVPS[1] * (8 * 0.01 / ACT) * a[1];

        double Y_min = Y_PIA5 + Y_PRIA8;
        double Y_Pmin = Y_PIA5 + Y_PRIA8 + Y_VIA7;

        // 5. Výpočet FK_x_t dle podmínek
        double FK_PIA, FK_PRIA, FK_VIA;

        if (Y > Y_min)
        {
            FK_PIA = UFK[0] + Y_PIA7 + Math.Min(Y_PIA5, (Y_x[0] - Y_PIA7) * 0.7);
            FK_PRIA = UFK[1] + Y_PRIA7 + Math.Min(Y_PRIA8, (Y_x[1] - Y_PRIA7) * 0.75);
            FK_VIA = UFK[2] + Y - Y_Pmin - Math.Min(Y_PIA5, (Y_x[0] - Y_PIA7) * 0.7) - Math.Min(Y_PRIA8, (Y_x[1] - Y_PRIA7) * 0.75);
        }
        else if (Y <= Y_min && Y > Y_Pmin)
        {
            FK_PIA = UFK[0] + Y_PIA7;
            FK_PRIA = UFK[1] + Y_PRIA7;
            FK_VIA = UFK[2] + Y - Y_Pmin;
        }
        else if (Y <= Y_Pmin && Y > 0)
        {
            FK_PIA = UFK[0] + Y * (Y_PIA7 / Y_Pmin);
            FK_PRIA = UFK[1] + Y * (Y_PRIA7 / Y_Pmin);
            FK_VIA = UFK[2];
        }
        else
        {
            FK_PIA = UFK[0] + Y_x[0];
            FK_PRIA = UFK[1] + Y_x[1];
            FK_VIA = UFK[2] + Y_x[2];
        }

        // Výstup výsledků
        Console.WriteLine($"FK_PIA: {FK_PIA}, FK_PRIA: {FK_PRIA}, FK_VIA: {FK_VIA}");
    }

    private double Sum(double[] array)
    {
        double sum = 0;
        foreach (double num in array)
        {
            sum += num;
        }
        return sum;
    }
}


public class FundCalculations3
{
    // Výstupní hodnoty výpočtů
    public double FK_Total; // Celkový fondový kapitál
    public double NAVPS_PIA, NAVPS_PRIA, NAVPS_VIA; // NAVPS pro jednotlivé typy akcií
    public double div_PIA, div_PRIA, div_VIA; // Kumulované dividendy
    public double a_PIA, a_PRIA, a_VIA; // Počet akcií každého typu
    public double ACT; // Počet dnů v aktuálním roce

    // Mezivýpočty
    public double UFK_PIA, UFK_PRIA, UFK_VIA; // Upravený fondový kapitál
    public double PFK_PIA, PFK_PRIA, PFK_VIA; // Podíly na celkovém UFK

    // Parametry zadané ve specifikaci
    public double PIAMax1Proc { get; set; } // 7% limit pro PIA (1. případ)
    public double PIAMax2Proc { get; set; } // 12% limit pro PIA (2. případ)
    public double PIAPrevodDoVIA { get; set; } // 30% převod přebytku do VIA
    
    public double PRIAMax1Proc { get; set; } // 7% limit pro PRIA (1. případ)
    public double PRIAMax2Proc { get; set; } // 15% limit pro PRIA (2. případ)
    public double PRIAPrevodDoVIA { get; set; } // 25% převod přebytku do VIA
    
    public double VIAMax1Proc { get; set; } // 7% limit pro VIA
    
    public int n { get; set; } // Počet uplynulých dnů referenčního období

    public FundCalculations3(
        double fk_total, double navps_pia, double navps_pria, double navps_via,
        double div_pia, double div_pria, double div_via,
        double a_pia, double a_pria, double a_via,
        double act, Parameters parameters)
    {
        FK_Total = fk_total;
        NAVPS_PIA = navps_pia;
        NAVPS_PRIA = navps_pria;
        NAVPS_VIA = navps_via;
        div_PIA = div_pia;
        div_PRIA = div_pria;
        div_VIA = div_via;
        a_PIA = a_pia;
        a_PRIA = a_pria;
        a_VIA = a_via;
        ACT = act;

        PIAMax1Proc = parameters.PIAMax1Proc / 100;
        PIAMax2Proc = parameters.PIAMax2Proc / 100;
        PIAPrevodDoVIA = parameters.PIAPrevodDoVIA / 100;
        PRIAMax1Proc = parameters.PRIAMax1Proc / 100;
        PRIAMax2Proc = parameters.PRIAMax2Proc / 100;
        PRIAPrevodDoVIA = parameters.PRIAPrevodDoVIA / 100;
        VIAMax1Proc = parameters.VIAMax1Proc / 100;
        n = parameters.n;
    }

    public void Calculate()
    {
         if (a_PIA <= 0 || a_PRIA <= 0 || a_VIA <= 0)
        {
            Console.WriteLine("Chyba: Pro výpočet musí být emitovány všechny typy akcií");
            return;
        }

        // 1. Výpočet upraveného fondového kapitálu (UFK)
        // UFK = (NAVPS - dividendy) × počet akcií
        UFK_PIA = (NAVPS_PIA - div_PIA) * a_PIA;
        UFK_PRIA = (NAVPS_PRIA - div_PRIA) * a_PRIA;
        UFK_VIA = (NAVPS_VIA - div_VIA) * a_VIA;
        Console.WriteLine($"ufk_pia = {UFK_PIA}");
        Console.WriteLine($"ufk_pria = {UFK_PRIA}");
        Console.WriteLine($"ufk_via = {UFK_VIA}");

        double total_UFK = UFK_PIA + UFK_PRIA + UFK_VIA;

        // 2. Výpočet podílů jednotlivých fondů na celkovém UFK
        // PFK = podíl daného fondu na celkovém UFK
        PFK_PIA = total_UFK > 0 ? UFK_PIA / total_UFK : 0;
        PFK_PRIA = total_UFK > 0 ? UFK_PRIA / total_UFK : 0;
        PFK_VIA = total_UFK > 0 ? UFK_VIA / total_UFK : 0;
        Console.WriteLine($"pfk_pia = {PFK_PIA}");
        Console.WriteLine($"pfk_pria = {PFK_PRIA}");
        Console.WriteLine($"pfk_via = {PFK_VIA}");

        // 3. Výpočet celkového výnosu Y
        // Y = rozdíl mezi aktuálním a upraveným fondovým kapitálem
        double Y = FK_Total - total_UFK;
        Console.WriteLine($"y = {Y}");

        // 4. Rozdělení výnosu Y podle podílů jednotlivých fondů
        double Y_PIA = Y * PFK_PIA;
        double Y_PRIA = Y * PFK_PRIA;
        double Y_VIA = Y * PFK_VIA;
        Console.WriteLine($"y_pia = {Y_PIA}");
        Console.WriteLine($"y_pria = {Y_PRIA}");
        Console.WriteLine($"y_via = {Y_VIA}");

        // 5. Výpočet hypotetických výnosů při různých ročních výnosech
        // Y_x7 = výnos při 7% p.a.
        double Y_PIA7 = NAVPS_PIA * (0.07 * n / ACT) * a_PIA;
        double Y_PRIA7 = NAVPS_PRIA * (0.07 * n / ACT) * a_PRIA;
        double Y_VIA7 = NAVPS_VIA * (0.07 * n / ACT) * a_VIA;

        // Speciální hypotetické výnosy pro PIA (5%) a PRIA (8%)
        double Y_PIA5 = NAVPS_PIA * (0.05 * n / ACT) * a_PIA;
        double Y_PRIA8 = NAVPS_PRIA * (0.08 * n / ACT) * a_PRIA;
        
        Console.WriteLine($"y_pia7 = {Y_PIA7} (7% p.a.)");
        Console.WriteLine($"y_pria7 = {Y_PRIA7} (7% p.a.)");
        Console.WriteLine($"y_via7 = {Y_VIA7} (7% p.a.)");
        Console.WriteLine($"y_pia5 = {Y_PIA5} (5% p.a.)");
        Console.WriteLine($"y_pria8 = {Y_PRIA8} (8% p.a.)");

        // 6. Výpočet mezních hodnot pro rozhodování o redistribuci
        // Y_Pmin = součet 7% výnosů PIA a PRIA
        // Y_min = Y_Pmin + 7% výnos VIA
        double Y_Pmin = Y_PIA7 + Y_PRIA7;
        double Y_min = Y_Pmin + Y_VIA7;

        Console.WriteLine($"y_pmin = {Y_Pmin} (PIA7 + PRIA7)");
        Console.WriteLine($"y_min = {Y_min} (Pmin + VIA7)");

        // 7. Hlavní výpočet podle 4 případů
        double FK_PIA, FK_PRIA, FK_VIA;
        string pripad = string.Empty;

        if (Y > Y_min)
        {
            pripad = "1 (vysoký výnos)";
            // Případ 1: Vysoký výnos (Y > Y_min)
            
            // MIN [ Y_PIA5 ; ( Y_PIA - Y_PIA7 ) × 70 % ]
            double minPIA = Math.Min(Y_PIA5, (Y_PIA - Y_PIA7) * 0.7);
            // MIN [ Y_PRIA8 ; ( Y_PRIA - Y_PRIA7 ) × 75 % ]
            double minPRIA = Math.Min(Y_PRIA8, (Y_PRIA - Y_PRIA7) * 0.75);
            Console.WriteLine($"případ = {pripad}");
            Console.WriteLine($"min_pia = {minPIA}");
            Console.WriteLine($"min_pria = {minPRIA}");

            // Základní výpočet FK s garantovaným a podmíněným výnosem
            FK_PIA = UFK_PIA + Y_PIA7 + minPIA;
            FK_PRIA = UFK_PRIA + Y_PRIA7 + minPRIA;
            FK_VIA = UFK_VIA + Y - Y_Pmin - minPIA - minPRIA;
            
            // Kontrola maximálního růstu PIA (PIAMax1Proc = 7%)
            // Podmínka: Pokud růst PIA přesáhl 7% z UFK_PIA
            if ((FK_PIA - UFK_PIA) > PIAMax1Proc * UFK_PIA)
            {
                // Přebytek = skutečný růst - maximální povolený růst
                double prebytek = (FK_PIA - UFK_PIA) - PIAMax1Proc * UFK_PIA;
                // Omezení FK_PIA na maximální povolenou hodnotu
                FK_PIA = UFK_PIA + PIAMax1Proc * UFK_PIA;
                // Převod přebytku do VIA (30%)
                FK_VIA += prebytek * PIAPrevodDoVIA;

                Console.WriteLine($"lim_pia: přebytek {prebytek}, převedeno do via: {prebytek * PIAPrevodDoVIA}");
            }
            
            // Stejná kontrola pro PRIA (PRIAMax1Proc = 7%)
            if ((FK_PRIA - UFK_PRIA) > PRIAMax1Proc * UFK_PRIA)
            {
                double prebytek = (FK_PRIA - UFK_PRIA) - PRIAMax1Proc * UFK_PRIA;
                FK_PRIA = UFK_PRIA + PRIAMax1Proc * UFK_PRIA;
                FK_VIA += prebytek * PRIAPrevodDoVIA; // 25% přebytku do VIA

                Console.WriteLine($"lim_pria: přebytek {prebytek}, převedeno do via: {prebytek * PRIAPrevodDoVIA}");
            }
            
            // Kontrola maximálního růstu VIA (VIAMax1Proc = 7%)
            if ((FK_VIA - UFK_VIA) > VIAMax1Proc * UFK_VIA)
            {
                double prebytek = (FK_VIA - UFK_VIA) - VIAMax1Proc * UFK_VIA;
                FK_VIA = UFK_VIA + VIAMax1Proc * UFK_VIA;
                // Redistribuce přebytku do PIA a PRIA podle jejich podílů
                FK_PIA += prebytek * PFK_PIA;
                FK_PRIA += prebytek * PFK_PRIA;

                Console.WriteLine($"lim_via: přebytek {prebytek}, redistribuováno: pia={prebytek * PFK_PIA}, pria={prebytek * PFK_PRIA}");
            }
        }
        else if (Y <= Y_min && Y > Y_Pmin)
        {
            // Případ 2: Střední výnos
            FK_PIA = UFK_PIA + Y_PIA7;
            FK_PRIA = UFK_PRIA + Y_PRIA7;
            FK_VIA = UFK_VIA + (Y - Y_Pmin);
            
            // Aplikace maximálních limitů
            if ((FK_PIA - UFK_PIA) > PIAMax2Proc * UFK_PIA)
            {
                double prebytek = (FK_PIA - UFK_PIA) - PIAMax2Proc * UFK_PIA;
                FK_PIA = UFK_PIA + PIAMax2Proc * UFK_PIA;
                FK_VIA += prebytek * PIAPrevodDoVIA;

                 Console.WriteLine($"lim_pia: přebytek {prebytek}, převedeno do via: {prebytek * PIAPrevodDoVIA}");
            }
            
            if ((FK_PRIA - UFK_PRIA) > PRIAMax2Proc * UFK_PRIA)
            {
                double prebytek = (FK_PRIA - UFK_PRIA) - PRIAMax2Proc * UFK_PRIA;
                FK_PRIA = UFK_PRIA + PRIAMax2Proc * UFK_PRIA;
                FK_VIA += prebytek * PRIAPrevodDoVIA;

                Console.WriteLine($"lim_pria: přebytek {prebytek}, převedeno do via: {prebytek * PRIAPrevodDoVIA}");
            }
        }
        else if (Y <= Y_Pmin && Y > 0)
        {
            pripad = "3 (nízký výnos)";
            // Případ 3: Nízký výnos
            FK_PIA = UFK_PIA + Y * (Y_PIA7 / Y_Pmin);
            FK_PRIA = UFK_PRIA + Y * (Y_PRIA7 / Y_Pmin);
            FK_VIA = UFK_VIA;

            Console.WriteLine($"případ = {pripad}");
        }
        else
        {
            pripad = "4 (záporný výnos)";
            // Případ 4: Záporný výnos
            FK_PIA = UFK_PIA + Y_PIA;
            FK_PRIA = UFK_PRIA + Y_PRIA;
            FK_VIA = UFK_VIA + Y_VIA;

            Console.WriteLine($"případ = {pripad}");
        }

        Console.WriteLine($"Výsledné hodnoty: PIA={FK_PIA}, PRIA={FK_PRIA}, VIA={FK_VIA}");
    }
}

public class Parameters
{
    public double PIAMax1Proc { get; set; }
    public double PIAMax2Proc { get; set; } 
    public double PIAPrevodDoVIA { get; set; } 
    public double PRIAMax1Proc { get; set; }
    public double PRIAMax2Proc { get; set; } 
    public double PRIAPrevodDoVIA { get; set; }
    public double VIAMax1Proc { get; set; }
    public int n { get; set; } 

    public Parameters(double piaMax1, double piaMax2, double piaPrevod,
                     double priaMax1, double priaMax2, double priaPrevod,
                     double viaMax1, int days)
    {
        PIAMax1Proc = piaMax1;
        PIAMax2Proc = piaMax2;
        PIAPrevodDoVIA = piaPrevod;
        PRIAMax1Proc = priaMax1;
        PRIAMax2Proc = priaMax2;
        PRIAPrevodDoVIA = priaPrevod;
        VIAMax1Proc = viaMax1;
        n = days;
    }
}
