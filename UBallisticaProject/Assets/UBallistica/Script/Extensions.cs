//                                  ┌∩┐(◣_◢)┌∩┐
//                                                                              \\
// Extensiones.cs (11/10/20016)                                              	\\
// Autor: Antonio Mateo (Moon Pincho) 									        \\
// Descripcion:  Extension para dibujar la trajectoria   						\\
// Fecha Mod:       (11/10/20016)                                               \\
// Ultima Mod:  Release inicial      											\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
using System.Collections;
#endregion

namespace MoonPincho.Extensiones
{
    public static class Extensiones
    {
        #region Debugs Extensiones
        /// <summary>
        /// <para>Crea un circulo</para>
        /// </summary>
        /// <param name="transform">Posicion del circulo</param>
        /// <param name="color">Color del circulo</param>
        /// <param name="radio">Radio del circulo</param>
        /// <param name="resolucion">Resoculion del circulo (100f)</param>
        /// <param name="fOV">360 </param>
        /// <param name="startEnOriginal">Empieza en el punto original</param>
        /// <param name="endEnOriginal">Termina en el punto original</param>
        /// <param name="dibujarBordes">Dibuja los bordes del circulo</param>
        public static void DebugCrearCirculo(this Transform transform,
                Color color,
                float radio = 5f,
                float resolucion = 100f,
                float fOV = 360,
                bool startEnOriginal = true,
                bool endEnOriginal = true,
                bool dibujarBordes = false)
        {
            Quaternion rot_a = Quaternion.AngleAxis(-(fOV * 0.5f), transform.up);
            Vector3 origin = transform.position;
            Vector3 startCorner = origin;
            Vector3 previousCorner = startCorner;

            for (int i = 0; i < resolucion + 1; i++)
            {

                float angle = fOV / 180;
                float cornerAngle = ((rot_a.eulerAngles.y + transform.eulerAngles.y) * Mathf.Deg2Rad) + (angle * Mathf.PI / (float)resolucion) * i;

                Vector3 currentCorner = new Vector3((Mathf.Sin(cornerAngle) * radio), 0, (Mathf.Cos(cornerAngle) * radio)) + origin;

                if (!(i == 0 && !startEnOriginal)) Debug.DrawLine(currentCorner, previousCorner, color);

                if (dibujarBordes) Debug.DrawLine(origin, currentCorner, color);

                previousCorner = currentCorner;
            }
            if (endEnOriginal) Debug.DrawLine(previousCorner, origin, color);
        }

        /// <summary>
        /// <para>Crea una linea</para>
        /// </summary>
        /// <param name="transform">Posicion de la linea</param>
        /// <param name="color">Color de la linea</param>
        /// <param name="radio">Radio de la linea</param>
        /// <param name="resolucion">Resolucion de la linea 100f</param>
        /// <param name="fOV">360</param>
        /// <param name="floatEnY">Control de la Y</param>
        /// <param name="startEnOriginal">Empieza en el punto original</param>
        /// <param name="endEnOriginal">Termina en el punto original</param>
        public static void CrearLineaCirculo(this Transform transform,
                Color color,
                float radio = 5f,
                float resolucion = 100f,
                float fOV = 360,
                float floatEnY = 1f,
                bool startEnOriginal = false,
                bool endEnOriginal = false)
        {
            LineRenderer lineRenderer = transform.GetComponent<LineRenderer>();
            if (lineRenderer)
            {
                Quaternion rot_a = Quaternion.AngleAxis(-(fOV * 0.5f), transform.up);
                Vector3 origin = transform.position + new Vector3(0, floatEnY);
                Vector3 startCorner = origin;
                Vector3 previousCorner = startCorner;

                Color startColor = color;
                Color endColor = startColor;
                lineRenderer.SetColors(startColor, endColor);
                int length = 0;

                if (startEnOriginal)
                {
                    lineRenderer.SetVertexCount(1);
                    lineRenderer.SetPosition(0, previousCorner);
                    length++;
                }

                for (int i = 0; i < resolucion + 2; i++)
                {

                    float angle = fOV / 180;
                    float cornerAngle = ((rot_a.eulerAngles.y + transform.eulerAngles.y) * Mathf.Deg2Rad) + (angle * Mathf.PI / (float)resolucion) * i;

                    Vector3 currentCorner = new Vector3((Mathf.Sin(cornerAngle) * radio), 0, (Mathf.Cos(cornerAngle) * radio)) + origin;

                    if (!(i == 0 && !startEnOriginal))
                    {
                        lineRenderer.SetVertexCount(length + 1);
                        lineRenderer.SetPosition(length, previousCorner);
                        length++;
                    }


                    previousCorner = currentCorner;
                }
                if (endEnOriginal) lineRenderer.SetPosition(length + 1, origin);
            }
        }

        /// <summary>
        /// <para>Crea la trayectoria y el circulos</para>
        /// </summary>
        /// <param name="transform">Posicion inicial</param>
        /// <param name="color">Color</param>
        /// <param name="radio">Radio</param>
        /// <param name="resolucion">Resolucion de la trayectorio</param>
        /// <param name="fOV">360</param>
        /// <param name="floatEnY">Control de la Y</param>
        /// <param name="startEnOriginal">Empieza en el punto original</param>
        /// <param name="endEnOriginal">Termina en el punto original</param>
        /// <param name="segundosDeEspera">Segundos antes de ejecucion</param>
        /// <returns></returns>
        public static IEnumerator CrearLineaCirculoCoRoutine(this Transform transform,
                Color color,
                float radio = 5f,
                float resolucion = 100f,
                float fOV = 360,
                float floatEnY = 1f,
                bool startEnOriginal = false,
                bool endEnOriginal = false,
                float segundosDeEspera = .5f)
        {
            transform.CrearLineaCirculo(color, radio, resolucion, fOV, floatEnY, startEnOriginal, endEnOriginal);
            yield return new WaitForSeconds(segundosDeEspera);
        }
        #endregion
    }
}