//                                  ┌∩┐(◣_◢)┌∩┐
//																				\\
// ArcBallistica.cs (30/03/2017)												\\
// Autor: Antonio Mateo (Moon Antonio) 									        \\
// Descripcion:		Generador del arco de la ballistica							\\
// Fecha Mod:		30/03/2017													\\
// Ultima Mod:		Version Inicial												\\
//******************************************************************************\\

#region Librerias
using UnityEngine;
#endregion

namespace MoonAntonio
{
	/// <summary>
	/// <para>Generador del arco de la ballistica</para>
	/// </summary>
	[RequireComponent(typeof(LineRenderer))]
	public class ArcBallistica : MonoBehaviour 
	{
		#region Variables Publicas
		/// <summary>
		/// <para>Velocidad del arco</para>
		/// </summary>
		public float velocidad = 0f;							// Velocidad del arco
		/// <summary>
		/// <para>Angulo del arco</para>
		/// </summary>
		public float angulo = 0f;								// Angulo del arco
		/// <summary>
		/// <para>Calidad visual del arco</para>
		/// </summary>
		public int calidadArc = 0;								// Calidad visual del arco
		#endregion

		#region Variables Privadas
		/// <summary>
		/// <para>LineRenderer del arco.</para>
		/// </summary>
		private LineRenderer lr = null;							// LineRenderer del arco
		/// <summary>
		/// <para>Fuerza de gravedad del eje Y</para>
		/// </summary>
		private float g = 0f;                                   // Fuerza de gravedad del eje Y
		/// <summary>
		/// <para>Angulo radiano del arco.</para>
		/// </summary>
		private float angRadi = 0.0f;							// Angulo radiano del arco
		#endregion

		#region Init
		/// <summary>
		/// <para>Inicializador de <see cref="ArcBallistica"/>.</para>
		/// </summary>
		private void Awake()// Inicializador de <see cref="ArcBallistica"/>
		{
			// Asignamos el LineRenderer
			lr = this.GetComponent<LineRenderer>();
			g = Mathf.Abs(Physics2D.gravity.y);
		}
		
		/// <summary>
		/// <para>Init de <see cref="ArcBallistica"/>.</para>
		/// </summary>
		private void Start()// Init de <see cref="ArcBallistica"/>
		{
			RenderizarArco();
		}
		#endregion

		#region GUI
		/// <summary>
		/// <para>Renderiza el arco</para>
		/// </summary>
		private void RenderizarArco()// Renderiza el arco
		{
			lr.SetVertexCount(calidadArc + 1);
			lr.SetPositions(CalularArrayArc());
		}
		#endregion

		#region Funcionalidad
		/// <summary>
		/// <para>Calcula el array del arco</para>
		/// </summary>
		private Vector3[] CalularArrayArc()// Calcula el array del arco
		{
			Vector3[] array = new Vector3[calidadArc + 1];

			// Calcular el angulo radiano y la distancia max
			angRadi = Mathf.Deg2Rad * angulo;
			float distanciaMax = (velocidad * velocidad * Mathf.Sin(2 * angRadi)) / g;

			for (int n = 0; n <= calidadArc; n++)
			{
				float i = (float)n / (float)calidadArc;
				array[n] = CalcularPuntoArc(i,distanciaMax);
			}

			return array;
		}

		/// <summary>
		/// <para>Calcula un punto del arco.</para>
		/// </summary>
		/// <param name="punto">Punto del arco</param>
		/// <param name="distanciaMax">Distancia maxima.</param>
		/// <returns></returns>
		private Vector3 CalcularPuntoArc(float punto, float distMax)// Calcula un punto del arco
		{
			float x = punto * distMax;
			float y = x * Mathf.Tan(angRadi) - ((g * x * x) / (2 * velocidad * velocidad * Mathf.Cos(angulo) * Mathf.Cos(angulo)));
			return new Vector3(x, y);
		}
		#endregion
	}
}
