using System.Collections;
using System.Collections.Generic;

public class DataCalucalculator
{
    public static float Damage(float damage,float block)
    {
        if (damage <= block)
        {
            return 1;
        }
        float final;
        final = (damage - block)*0.8f;
        return final;
    }
}
