using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilitiesTm
{
    public static float FloatRemap(float val, float in1, float in2, float out1, float out2)
    {
        return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
    }

    public static Color32[] ChangeColors(Color32[] colorArray, Color32 changeToColor)
    {
        for (int i = 0; i < colorArray.Length; i++)
        {
            colorArray[i] = changeToColor;
        }
        return colorArray;
    }

    public static string FloaTTimeToString(float seconds)
    {
        string strSeconds = (seconds % 60).ToString("00");
        string strMinutes = (seconds/60).ToString("00");
        return strMinutes + ":" + strSeconds;
    }

    public static int sgn(int x)
    {
        return (x > 0) ? 1 : (x < 0) ? -1 : 0;
    }

    public static Texture2D Bresenham(Texture2D texture2D, int xstart, int ystart, int xend, int yend, Color32[] colors)
    /*--------------------------------------------------------------
     * Bresenham-Algorithmus: Linien auf Rastergeräten zeichnen
     *
     * Eingabeparameter:
     *    int xstart, ystart        = Koordinaten des Startpunkts
     *    int xend, yend            = Koordinaten des Endpunkts
     *
     * Ausgabe:
     *    void SetPixel(int x, int y) setze ein Pixel in der Grafik
     *         (wird in dieser oder aehnlicher Form vorausgesetzt)
     *         
     * Quelle:
     *    https://de.wikipedia.org/wiki/Bresenham-Algorithmus#C-Implementierung
     *---------------------------------------------------------------
     */
    {
        int x, y, t, dx, dy, incx, incy, pdx, pdy, ddx, ddy, deltaslowdirection, deltafastdirection, err;

        /* Entfernung in beiden Dimensionen berechnen */
        dx = xend - xstart;
        dy = yend - ystart;

        /* Vorzeichen des Inkrements bestimmen */
        incx = sgn(dx);
        incy = sgn(dy);
        if (dx < 0) dx = -dx;
        if (dy < 0) dy = -dy;

        /* feststellen, welche Entfernung größer ist */
        if (dx > dy)
        {
            /* x ist schnelle Richtung */
            pdx = incx; pdy = 0;    /* pd. ist Parallelschritt */
            ddx = incx; ddy = incy; /* dd. ist Diagonalschritt */
            deltaslowdirection = dy; deltafastdirection = dx;   /* Delta in langsamer Richtung, Delta in schneller Richtung */
        }
        else
        {
            /* y ist schnelle Richtung */
            pdx = 0; pdy = incy; /* pd. ist Parallelschritt */
            ddx = incx; ddy = incy; /* dd. ist Diagonalschritt */
            deltaslowdirection = dx; deltafastdirection = dy;   /* Delta in langsamer Richtung, Delta in schneller Richtung */
        }

        /* Initialisierungen vor Schleifenbeginn */
        x = xstart;
        y = ystart;
        err = deltafastdirection / 2;
        //SetPixel(x, y);
        texture2D.SetPixels32(x, y, 3, 3, colors);
        //texture2D.SetPixel(x, y, Color.black);

        /* Pixel berechnen */
        for (t = 0; t < deltafastdirection; ++t) /* t zaehlt die Pixel, deltafastdirection ist Anzahl der Schritte */
        {
            /* Aktualisierung Fehlerterm */
            err -= deltaslowdirection;
            if (err < 0)
            {
                /* Fehlerterm wieder positiv (>=0) machen */
                err += deltafastdirection;
                /* Schritt in langsame Richtung, Diagonalschritt */
                x += ddx;
                y += ddy;
            }
            else
            {
                /* Schritt in schnelle Richtung, Parallelschritt */
                x += pdx;
                y += pdy;
            }
            //SetPixel(x, y);
            texture2D.SetPixels32(x, y, 3, 3, colors);
            //texture2D.SetPixel(x, y, Color.black);
        }
        return texture2D;
    } /* gbham() */


}
