// То, что не нужно, но жалко удалять

/// <summary>
		/// Метод Зеёделя нахождения корней линейного алгебраического уравнения
		/// matrix[0,0]*x₁+matrix[0,1]*x₂+...+matrix[0,n]*x_n=free_coef[0]
		/// </summary>
		/// <param name="coef">матрица неизвестных</param>
		/// <param name="free_coef">массив свободных членов</param>
		/// <param name="precision">степень точности (0,1^precision)</param>
		/// <returns></returns>

		static double[] MethodIterationImprovedForSpline4(double[,] coef, double[] free_coef, double precision)
		{
			double[] x = new double[9];
			double[] old_x = new double[9];
			int s; //номер вычисляемого в текущей итерации коэффициента x_s
			for (int i = 0; i < 9; i++) x[i] = 1;

			while (true) {
				x.CopyTo(old_x, 0);

				for (int i = 0; i < 9; i++) {
					switch (i) {
						case 0: s = 0; break;
						case 1: s = 5; break;
						case 2: s = 8; break;
						case 3: s = 3; break;
						case 4: s = 6; break;
						case 5: s = 2; break;
						case 6: s = 4; break;
						case 7: s = 1; break;
						case 8: s = 7; break;
						default: s = 666; break;
					}
					x[s] = 0;

					for (int j = 0; j < 9; j++)
						if (j != s) x[s] += coef[i, j] * x[j];

					x[s] -= free_coef[i];
					x[s] /= -coef[i, s];
				}

				double Difference = 0;
				for (int i = 0; i < 9; i++) {
					Difference += Sqr(old_x[i] - x[i]);
				}
				Difference = Math.Pow(Difference, (double)1 / 9);
				if (Difference < precision) break;
			}
			return x;

		}