using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//dodanie nowej przestrzeni nazw dla "potrzeb" kontrolki Chart
using System.Windows.Forms.DataVisualization.Charting;

namespace PRojekt32_Safonoc58820
{
    public partial class Formularz : Form
    {
        public Formularz()
        {
            InitializeComponent();
        }

        private void btnTabelarycznaWizualizacja_Click(object sender, EventArgs e)
        {//deklaracja zmiennych dla przechowania danych wejsciowych pobranych z kontrolek 
            //formularza
            float rsXd, rsXg, rsH, rsEps;
            //pobranie danych wejściowych
            if (!rsPobierzDaneWejściowe(out rsEps, out rsXd, out rsXg, out rsH))
                //przerwanie obsługo zdarzenia Click
                return;

            //utworzenie tabeli zmian wartości szeregu
            //deklaracja zmiennej tablicowanie
            float[,] rsTWS; //rsTWS
            //wywołanie metody utworzenia tabeli zmian wartożci szeregu 
            rsTablicowanieWartościSzeregu(rsXd, rsXg, rsH, rsEps, out rsTWS);
            //wypełnienie kontrolki DataGridView danymi z tablicowania wartości szeregu
            //odsłonięcie kontrolki DataGridView
            dgvTabelaWartościSzeregu.Visible = true;
            //ewentualne ukrycie innych kontrolek
            lblSzeregPotęgowy.Visible = false;
            txtObliczonaSumaSzeregu.Visible = false;
            
            //. . .

            //wyzerowanie danych "satrych w kontrolce DataGridView"
            dgvTabelaWartościSzeregu.Rows.Clear();
            //wycentrowanie zapisu danych w kolumnach kontrolki DataGridView
            dgvTabelaWartościSzeregu.Columns[0].DefaultCellStyle.Alignment = 
                DataGridViewContentAlignment.MiddleCenter;

            dgvTabelaWartościSzeregu.Columns[1].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgvTabelaWartościSzeregu.Columns[2].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            //przepisanie danych z tablicy rsTWS do kontrolki DataGridView
            for(int i = 0; i < rsTWS.GetLength(0); i++)
            {
                //dodanie do kontrolki DataGridView nowego wiersza 
                dgvTabelaWartościSzeregu.Rows.Add();
                //wpisanie wartosci rsX
                dgvTabelaWartościSzeregu.Rows[i].Cells[0].Value =
                    string.Format("{0:F2}", rsTWS[i, 0]);
                //wpisanie wartosci szeregu S(rsX)
                dgvTabelaWartościSzeregu.Rows[i].Cells[1].Value =
                    string.Format("{0:F3}", rsTWS[i, 1]);
                //wpisanie licznika zsumowanych wyrazow szeregu
                dgvTabelaWartościSzeregu.Rows[i].Cells[2].Value =
                    string.Format("{0}", rsTWS[i, 2]);
            }
            //stawienie stanu braku aktywnosci dla przycisku polecen
            btnTabelarycznaWizualizacja.Enabled = false;
            chtWykresSzeregu.Visible = false;
            btnGraficznaWizualizacja.Enabled = true;
            lblWykresZmian.Visible = false;
            formatowanieKontrolkiDataGridViewToolStripMenuItem.Enabled = true;
            zapiszTablicęDataGridView.Enabled = true;
            formatowanieKontrolkiChartwykresuToolStripMenuItem.Enabled = false;

            btnKolorLinii.Enabled = false;
            btnKolorTlaObszWyk.Enabled = false;
            txtKolotTła.Enabled = false;
            txtWziernikKolLin.Enabled = false;
            cmbTypWykresu.Enabled = false;
            odczytajTablicęWartościSzereguZPlikuToolStripMenuItem.Enabled = true;
            groupBox1.Enabled = false;



        }

        //deklaracja bliźniaczej metody
        void rsTablicowanieWartościSzeregu(ref float[,] rsTWS, float rsXd, float rsXg, float rsH, float rsEps)
        {
            //deklaracje pomocnicze
            float rsX;
            int i;
            ushort rsLiczbaZsumowanychWyrazówSzeregu;
            for(rsX = rsXd, i = 0; i <rsTWS.GetLength(0); i++, rsX = rsXd + i * rsH)
            {
                rsTWS[i, 0] = rsX;
                rsTWS[i, 1] = rsObliczenieSumySzeregu(rsX, rsEps, out rsLiczbaZsumowanychWyrazówSzeregu);
            }
        }
        void rsTablicowanieWartościSzeregu(float rsXd, float rsXg, float rsH, float rsEps,out 
            float [,] rsTWS)
        {
            //oblicyenie licybz wierszy egzemplarza tablicy rsTWS
            int n = (int)(Math.Abs(rsXg - rsXd) / rsH) + 1;
            rsTWS = new float[n, 3];
            //deklaracja pomocznice
            float rsX;
            ushort rsLicznik;
            int i; //numer podprzedzialu
            //tablicowanie wartosci szeregu
            for(rsX = rsXd, i = 0; i < n; i++, rsX = rsXd + i * rsH)
            {
                rsTWS[i, 0] = rsX;
                rsTWS[i, 1] = rsObliczenieSumySzeregu(rsX, rsEps, out rsLicznik);
                rsTWS[i, 2] = rsLicznik;
            }
        }

        static float rsObliczenieSumySzeregu(float rsX, float rsEps, out ushort k)
        {// deklaracje lokalne
            float W, SumaSzeregu;
            //ustalenie poczatkowega stanu obliczen
            k = 0;
            W = 1.0F;
            SumaSzeregu = 0.0F; //suma szeregu jest rowna wyrazowi stalemu
            // iteracyjne obliczanie szeregu potegowego
            do
            {
                SumaSzeregu = SumaSzeregu + W;
                k++;
                W = W * (-rsX / k);
            } while (Math.Abs(W) > rsEps);
            // zwrotne przekazanie wartosci obliczonej sumy szeregu potegowego
            return SumaSzeregu;
        }

        bool rsPobierzDaneWejściowe(out float rsEps, out float rsXd, out float rsXg,
            out float rsH)
        {
            // ustawienie domyślnych wartości dla parametrów wyjściowych, gdy
            // chcemy "zapalać" kontrolkę errorProvider1 dla sygnalizacji błędów
            rsEps = 0.0F; rsXd = 0.0F; rsXg = 0.0F; rsH = 0.0F;


            //pobranie dokładności obliczeń rsEps 
            // sprawdzenie, czy została wpisana dokładności obliczeń rsEps
            if (string.IsNullOrEmpty(txtWartośćEps.Text))
            {// "zapalenie" kontrolki errorProvider (sygnalizacja błędu)
                errorProvider1.SetError(txtWartośćEps,
                    "ERROR: musisz podać dokładności obliczeń rsEps!");

                    return false;/*Zakończenie pobierania danych wejściowych i 
                                  zwrotne przekazanie wartości "false" */
            }
            else
                errorProvider1.Dispose(); // "zgaszenie" kontrolki errorProvider1
            //pobranie dokładności obliczeń rsEps
            if (!float.TryParse(txtWartośćEps.Text, out rsEps))
            {
                errorProvider1.SetError(txtWartośćEps, "ERROR: wystąpił" +
                    " niedozwolony znak w zapisie wartość rsEps ");
                return false; /* zakonczenie pobierania danych wejsciowych i
                               zwrotne przekazanie wartosci "false" */
            }
            else
                errorProvider1.Dispose();
            //sprawdzenie warunku wejsciowego dla dokladnosci obliczen rsEps

            if ((rsEps <= 0.0F) || (rsEps >= 1.0F))
            {
                errorProvider1.SetError(txtWartośćEps, "ERROR: dokładności obliczeń" +
                   " rsEps musi spełniać warunek wejściowy: 0.0 < rsEps < 1.0");
                return false; /*zakonczenie pobierania danzch wejsciowych i
                               zwrotne przekazanie wartosci false*/
            }
            else
                errorProvider1.Dispose(); //zgaszenie kontrolki errorProvider1

            //pobranie dolnej granicy przedzialu wartosci zmiennej rsX
            //sprawdzenie, czy zostala wpisana dolna granica przedzialu

            if (string.IsNullOrEmpty(txtWartośćXd.Text))
            {
                errorProvider1.SetError(txtWartośćXd, "ERROR: musisz" +
                    " podać wartość rsXd (dolnej granicy przedziału) ");
                return false; /* zakonczenie pobierania danych wejsciowych i
                               zwrotne przekazanie wartosci "false" */
            }
            else
                errorProvider1.Dispose();

            if (!float.TryParse(txtWartośćXd.Text, out rsXd))
            {
                errorProvider1.SetError(txtWartośćXd, "ERROR: wystąpił" +
                    " niedozwolony znak w zapisie wartość rsXd ");
                return false; /* zakonczenie pobierania danych wejsciowych i
                               zwrotne przekazanie wartosci "false" */
            }
            else
                errorProvider1.Dispose();

            //pobranie górnej granicy przedzialu wartosci zmiennej rsX
            //sprawdzenie, czy zostala wpisana dolna granica przedzialu

            if (string.IsNullOrEmpty(txtWartośćXg.Text))
            {
                errorProvider1.SetError(txtWartośćXg, "ERROR: musisz" +
                    " podać wartość rsXd (dolnej granicy przedziału) ");
                return false; /* zakonczenie pobierania danych wejsciowych i
                               zwrotne przekazanie wartosci "false" */
            }
            else
                errorProvider1.Dispose();

            if (!float.TryParse(txtWartośćXg.Text, out rsXg))
            {
                errorProvider1.SetError(txtWartośćXg, "ERROR: wystąpił" +
                    " niedozwolony znak w zapisie wartość rsXg ");
                return false; /* zakonczenie pobierania danych wejsciowych i
                               zwrotne przekazanie wartosci "false" */
            }
            else
                errorProvider1.Dispose();

            if (rsXd > rsXg)
            {
                errorProvider1.SetError(txtWartośćXg, "ERROR: dolna granica przedziału" +
                    " nie może być większa od górnej granicy przedziału wartość rsXg");

                return false;
            }
            else
                errorProvider1.Dispose();

            //pobranie kroku rsH
            //sprawdzenie, czy został wpisany przyrost rsH

            if (string.IsNullOrEmpty(txtWartośćPrzyrostu.Text))
            {
                errorProvider1.SetError(txtWartośćPrzyrostu, "ERROR: wystąpił" +
                    " niedozwolony znak w zapisie wartość rsH");
                return false; /* zakonczenie pobierania danych wejsciowych i
                               zwrotne przekazanie wartosci "false" */
            }
            else
                errorProvider1.Dispose();

            if (!float.TryParse(txtWartośćPrzyrostu.Text, out rsH))
            {
                errorProvider1.SetError(txtWartośćPrzyrostu, "ERROR: przyrost rsH (krok zmian zmiennej wartości rsX)" +
                  " powinien spełniać warunek: 0 < rsH < 1");
                return false; /*zakonczenie pobierania danzch wejsciowych i
                               zwrotne przekazanie wartosci false*/
            }
            else
                errorProvider1.Dispose();

            if ((rsH <= 0) || (rsH >= 1))
            {
                errorProvider1.SetError(txtWartośćPrzyrostu, "ERROR: przyrost rsH (krok zmian zmiennej wartości rsX)" +
                   " powinien spełniać warunek: 0 < rsH < 1");
                return false; /*zakonczenie pobierania danzch wejsciowych i
                               zwrotne przekazanie wartosci false*/
            }
            else
                errorProvider1.Dispose(); //zgaszenie kontrolki errorProvider1


            txtWartośćXd.Enabled = false;
            txtWartośćXg.Enabled = false;
            txtWartośćEps.Enabled = false;
            txtWartośćPrzyrostu.Enabled = false;

            return true; //zwrotne przekazanie informacji, nie było błędu przy
                        //pobieraniu danych wejściowych z formularza

        }

        bool rsPobierzDaneWejściowe2(out float rsX, out float rsEps)
        {

            rsX = 0.0f; rsEps = 0.0F;


            //pobranie dokładności obliczeń rsX
            // sprawdzenie, czy została wpisana dokładności obliczeń rsX
            if (string.IsNullOrEmpty(txtWartośćX.Text))
            {// "zapalenie" kontrolki errorProvider (sygnalizacja błędu)
                errorProvider1.SetError(txtWartośćX,
                    "ERROR: musisz podać rsX!");

                return false;/*Zakończenie pobierania danych wejściowych i 
                                  zwrotne przekazanie wartości "false" */
            }
            else
                errorProvider1.Dispose(); // "zgaszenie" kontrolki errorProvider1

            if (!float.TryParse(txtWartośćX.Text, out rsX))
            {//sygnalizacja o błędach 
                errorProvider1.SetError(txtWartośćX, "ERROR: w znaczeniu rsX musi być cyfra");

                return false;
            }
            else
                errorProvider1.Dispose();//zgaszenie kontrolki errorProvider

            //pobranie dokładności obliczeń rsEps 
            // sprawdzenie, czy została wpisana dokładności obliczeń rsEps
            if (string.IsNullOrEmpty(txtWartośćEps.Text))
            {// "zapalenie" kontrolki errorProvider (sygnalizacja błędu)
                errorProvider1.SetError(txtWartośćEps,
                    "ERROR: musisz podać dokładności obliczeń rsEps!");

                return false;/*Zakończenie pobierania danych wejściowych i 
                                  zwrotne przekazanie wartości "false" */
            }
            else
                errorProvider1.Dispose(); // "zgaszenie" kontrolki errorProvider1
            //pobranie dokładności obliczeń rsEps
            if (!float.TryParse(txtWartośćEps.Text, out rsEps))
            {
                errorProvider1.SetError(txtWartośćEps, "ERROR: wystąpił" +
                    " niedozwolony znak w zapisie wartość rsEps ");
                return false; /* zakonczenie pobierania danych wejsciowych i
                               zwrotne przekazanie wartosci "false" */
            }
            else
                errorProvider1.Dispose();
            //sprawdzenie warunku wejsciowego dla dokladnosci obliczen rsEps

            if ((rsEps <= 0.0F) || (rsEps >= 1.0F))
            {
                errorProvider1.SetError(txtWartośćEps, "ERROR: dokładności obliczeń" +
                   " rsEps musi spełniać warunek wejściowy: 0.0 < rsEps < 1.0");
                return false; /*zakonczenie pobierania danzch wejsciowych i
                               zwrotne przekazanie wartosci false*/
            }
            else
                errorProvider1.Dispose(); //zgaszenie kontrolki errorProvider1


            return true;
        }

        private void btnObliczSumęSzeregu_Click(object sender, EventArgs e)
        {//deklaracja zmiennych
            //pobranie danych wejsciowych 

           if(!rsPobierzDaneWejściowe2(out float rsX, out float rsEps))
                return; //bezwarunkowe wyjscie z metody

            float W, SumaSzeregu;
            //ustalenie poczatkowega stanu obliczen
            float k = 0;
            W = 1.0F;
            SumaSzeregu = 0.0F; //suma szeregu jest rowna wyrazowi stalemu
            // iteracyjne obliczanie szeregu potegowego
            do
            {
                SumaSzeregu = SumaSzeregu + W;
                k++;
                W = W * (-rsX / k);
            } while (Math.Abs(W) > rsEps);

            txtObliczonaSumaSzeregu.Text = string.Format("{0:0.000}", SumaSzeregu);

        }

        private void btnGraficznaWizualizacja_Click(object sender, EventArgs e)
        {
            //deklaracje zmiennych dla przechowywania danych wejsciowych
            float rsXd, rsXg, rsH, rsEps;
            //pobranie danych wejsciowych
            if (!rsPobierzDaneWejściowe(out rsEps, out rsXd, out rsXg, out rsH))
                //był błąd, to musimy przęrwać dalszą obsługę zdarzenia Click
                return;
            //wyznaczenie liczby wierszy dla tablicy wartości szeregu
            int n = (int)(Math.Abs(rsXg - rsXd) / rsH) + 1;
            //utworzenie egzemplarza tablicy wartości szeregu
            float[,] rsTWS = new float[n + 1, 2];
            //stablicowanie wartosci szeregu
            rsTablicowanieWartościSzeregu(ref rsTWS, rsXd, rsXg, rsH, rsEps);
            //odsłona kontrolki Chart
            chtWykresSzeregu.Visible = true;
            //ukrycice kontrolki DAtaGridView
            dgvTabelaWartościSzeregu.Visible = false;
            //lokalizacja i zwymiarowanie kontrolki Chart
            chtWykresSzeregu.Width = (int)(this.Width * 0.40F);
            chtWykresSzeregu.Height = (int)(this.Height * 0.45F);
            //ustalenie tytułu wykresu
            //chtWykresSzeregu.Titles.Add("Wykres zmian wartości szeregu");
            //Umieszczenie legendy rysunku pod rysunkiem
            //chtWykresSzeregu.Legends.FindByName("Legend1").Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            chtWykresSzeregu.Legends.FindByName("Legend1").Docking = Docking.Bottom;
            //ustalenie koloru dla tla wykresu
            chtWykresSzeregu.BackColor = Color.GreenYellow;
            //opis osi rsX oraz Y
            chtWykresSzeregu.ChartAreas[0].AxisX.Title = "Wartość zmiennej X";
            chtWykresSzeregu.ChartAreas[0].AxisY.Title = "Wartość szeregu S(X)";
            //sformatowanie opisu osi rsX
            chtWykresSzeregu.ChartAreas[0].AxisX.LabelStyle.Format = "{0:F2}";
            chtWykresSzeregu.ChartAreas[0].AxisY.LabelStyle.Format = "{0:F2}";
            //utworzenie serii danych dla punktow wykresu
            chtWykresSzeregu.Series.Clear();
            //dodanie nowej serii danych o numerze 0
            chtWykresSzeregu.Series.Add("Seria 0");
            //ustalenie nazwy dla dodanej serii danych
            chtWykresSzeregu.Series[0].Name = "Wartość szeregu potęgowego";
            //ustalenie rodzaju wykresu
            chtWykresSzeregu.Series[0].ChartType = SeriesChartType.Line;
            //ustalenie koloru linii wykresu
            chtWykresSzeregu.Series[0].Color = Color.Black;
            //ustalenie stylu linii wykresu 
            chtWykresSzeregu.Series[0].BorderDashStyle = ChartDashStyle.Solid;
            //ustalenie grudości linii
            chtWykresSzeregu.Series[0].BorderWidth = 2;
            //wpisanie współrzędnych punktów wykresu 
            for(int i = 0; i < rsTWS.GetLength(0); i++)
            {
                chtWykresSzeregu.Series[0].Points.AddXY(rsTWS[i, 0], rsTWS[i, 1]);
            }
            //ustalenioe stanu braku aktywnosci dla przycisku polocen Graficzna wizualizacja...
            btnGraficznaWizualizacja.Enabled = false;
            txtGrubośćLinii.Enabled = true;
            trbGrubośćLiniiWyk.Enabled = true;
            txtObliczonaSumaSzeregu.Visible = true;
            lblSzeregPotęgowy.Visible = true;
            btnObliczSumęSzeregu.Enabled = false;
            btnTabelarycznaWizualizacja.Enabled = true;
            lblWykresZmian.Visible = true;
            formatowanieKontrolkiDataGridViewToolStripMenuItem.Enabled = false;
            zapiszTablicęDataGridView.Enabled = false;
            formatowanieKontrolkiChartwykresuToolStripMenuItem.Enabled = true;

            btnKolorLinii.Enabled = true;
            btnKolorTlaObszWyk.Enabled = true;
            txtKolotTła.Enabled = true;
            txtWziernikKolLin.Enabled = true;
            cmbTypWykresu.Enabled = true;

            odczytajTablicęWartościSzereguZPlikuToolStripMenuItem.Enabled = false;
            groupBox1.Enabled = true;

        }

        private void trbGrubośćLiniiWyk_Scroll(object sender, EventArgs e)
        {
            //ustawienie grubości linii w bliżniaczej kontrolce
            txtGrubośćLinii.Text = trbGrubośćLiniiWyk.Value.ToString();
            /*
             ustawienie nowej grubości linii w kontrolce Chart dal serii danych serii 0
             */
            chtWykresSzeregu.Series[0].BorderWidth = trbGrubośćLiniiWyk.Value;
        }

        private void txtGrubośćLinii_TextChanged(object sender, EventArgs e)
        {
            int GrubośćLiniiWykr;
            if (!int.TryParse(txtGrubośćLinii.Text, out GrubośćLiniiWykr) || GrubośćLiniiWykr < 1 || GrubośćLiniiWykr > 10)
            {
                //zapalenie kontrolki ErrorProvider1
                errorProvider1.SetError(txtGrubośćLinii, "ERROR: wystąpił niezdowolny znak w zapisie grubość linii");

                return;
            }
            else
                errorProvider1.Dispose();
            
            trbGrubośćLiniiWyk.Value = GrubośćLiniiWykr;
            chtWykresSzeregu.Series[0].BorderWidth = trbGrubośćLiniiWyk.Value;
        }

        private void odczytajTablicęWartościSzereguZPlikuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
             deklaracja i utworzenie egzemplarza okna dialogowego dla wyboru pliku
             */

            OpenFileDialog OknoOdczytuPliku = new OpenFileDialog();
            //ustawienie filtrów plików, które mogą być pokazane w oknie dialogowym
            OknoOdczytuPliku.Filter = "txt files (*.txt)|*.txtAll Files (*.*)|*.*";
            //wybór filtru domyślnego
            OknoOdczytuPliku.FilterIndex = 1;
            //przywracenie bieżącego ktalogu po zamknięciu okna dialogowego
            OknoOdczytuPliku.RestoreDirectory = true;
            //domyślny wybór dysku i folderu w oknie dialogowym wyboru pliku
            OknoOdczytuPliku.InitialDirectory = "F:\\";
            //ustalenie tytułu okna dialogowego wyboru pliku
            OknoOdczytuPliku.Title = "Odczytanie (pobranie) danych z pliku";

            /* Sprawdzenie czy użytkownik wybrał przycisk OK
             i jeśli tak, to otwarcie pliku w trybie strumienia znaków */

            if (OknoOdczytuPliku.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamReader PlikZnakowy =
                    new System.IO.StreamReader(OknoOdczytuPliku.FileName);

                try
                {
                    //zerowanie wierszy danych kontrolki DataGridView
                    dgvTabelaWartościSzeregu.Rows.Clear();
                    //wycentrowanie zapisów w kolumnach kontrolki DataGridView

                    dgvTabelaWartościSzeregu.Columns[0].DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleCenter;
                    dgvTabelaWartościSzeregu.Columns[1].DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleCenter;
                    dgvTabelaWartościSzeregu.Columns[2].DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleCenter;

                    string rsWierszDanych; //dla przechowania wiersza danych wczytanego z pliku
                    string[] rsTablicaWierszy; //dla podzielonego wiersza danych na linie wierszy
                    while ((rsWierszDanych = PlikZnakowy.ReadLine()) != null)
                    {
                        rsTablicaWierszy = rsWierszDanych.Split(';');
                        rsTablicaWierszy[0].Trim();
                        rsTablicaWierszy[1].Trim();
                        rsTablicaWierszy[3].Trim();

                        dgvTabelaWartościSzeregu.Rows.Add(rsTablicaWierszy[0],
                            rsTablicaWierszy[1], rsTablicaWierszy[2]);
                    }

                    //odsłonięcie kontrolki DataGridView
                    dgvTabelaWartościSzeregu.Visible = true;
                }
                catch (Exception ex)
                {

                    MessageBox.Show("ERROR: nie można pobierać (wczytać) danych z pliku " +
                        "- wyświetlony komunikat: " + ex.Message);
                }
                finally
                {
                    PlikZnakowy.Close();
                    PlikZnakowy.Dispose();
                }

            }



        }

        private void zmianaCzcionkiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //dgvTabelaWartościSzeregu.Font

            FontDialog OknoZmianyCzcionki = new FontDialog();
            //zaznaczenie w OknoZmianyCzcionki aktywnych ustawień atrybutów czcionki

            OknoZmianyCzcionki.Font = dgvTabelaWartościSzeregu.Font;
            //wyświetle (uaktywnienie) okna dialogowego OknoZmianyCzcionki
            if (OknoZmianyCzcionki.ShowDialog() == DialogResult.OK)
            {
                //zmiana atrybutów czcionki w kontrolce dgvTabelaWartościSzeregu
                dgvTabelaWartościSzeregu.Font = OknoZmianyCzcionki.Font;
            }
        }

        private void zapiszTablicęDataGridView_Click(object sender, EventArgs e)
        {
            //deklaracja i zmiana egzemplarza dla zapisu pliku

            SaveFileDialog OknoZapisuPliku = new SaveFileDialog();

            //ustawienie filtru wyboru plików w oknie dialogowym
            OknoZapisuPliku.Filter = "txt files (*.txt)|*.txt|All files(*.*)| *.*";
            //wybór filtru dla naszego programu
            OknoZapisuPliku.FilterIndex = 1;//czyli filtr: *.txt
            //chcemy przywrocic biezacy folder po zamknieciu okna dialogowego
            OknoZapisuPliku.RestoreDirectory = true;
            //ustawienie wyboru dysku do zapisania tablicy wartosci
            OknoZapisuPliku.InitialDirectory = "F:\\";
            //ustalenie tytułuokna dialogowego OknoZAPISUpliku
            OknoZapisuPliku.Title = "Zapisanie tabeli wartości szeregu w pliku";
            // spradzenie, czy Użytkownik wybrał przycisk OK w oknie dialogowym OknoZapisuPliku
            if (OknoZapisuPliku.ShowDialog() == DialogResult.OK)
            {

                //otwarcie pliku do zapisu jako strumienia
                System.IO.StreamWriter PlikZnakowy =
                    new System.IO.StreamWriter(OknoZapisuPliku.FileName);

                //może zapisać
                try
                {

                    //wypisywanie dp pliku tabeli wartości z kontrolki DataGridView
                    for (int i = 0; i < dgvTabelaWartościSzeregu.Rows.Count; i++)
                    {
                        PlikZnakowy.Write(dgvTabelaWartościSzeregu.Rows[i].Cells[0].Value);

                        PlikZnakowy.Write(" ; ");

                        PlikZnakowy.Write(dgvTabelaWartościSzeregu.Rows[i].Cells[1].Value);

                        PlikZnakowy.Write(" ; ");

                        PlikZnakowy.WriteLine(dgvTabelaWartościSzeregu.Rows[i].Cells[2].Value);


                    }
                    //wyzerowanie kontrolki DataGridView
                    dgvTabelaWartościSzeregu.Rows.Clear();

                }
                catch (Exception ex)
                {//obsługa sygnalizowanego wyjątku

                    MessageBox.Show("ERROR: nie można wykonać operacji na " +
                        "pliku - (wyśletlony komunikat): --> " + ex.Message);
                }
                finally
                {
                    //zamkniecie pliku
                    PlikZnakowy.Close();
                    //zwolnienie zasobow zwiazanych plikiem PlikZnakowy
                    PlikZnakowy.Dispose();
                }
            }
        }

        private void zamknięcieFormularzaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void zakończenieDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnKolorTlaObszWyk_Click(object sender, EventArgs e)
        {

            ColorDialog KolorTła = new ColorDialog();

            //zaznaczenie  w KolorTła aktywnych ustawień atrybutów czcionki
            KolorTła.Color = chtWykresSzeregu.BackColor;

            //wyświetlenie okna dialogowego KolorTła
            if (KolorTła.ShowDialog() == DialogResult.OK)
            {//zmiana koloru 
                chtWykresSzeregu.BackColor = KolorTła.Color;
                lblWykresZmian.BackColor = KolorTła.Color;
                txtKolotTła.BackColor = KolorTła.Color;
            }
        }

        private void btnKolorLinii_Click(object sender, EventArgs e)
        {
            //            chtWykresSzeregu.Series[0].Color = Color.Black;
            ColorDialog KolorLiniiWykresu = new ColorDialog();

            //zaznaczenie w KolorLiniiWykresu aktywny kolor
            KolorLiniiWykresu.Color = chtWykresSzeregu.Series[0].Color;

            //wyświetlenie okna dialogowego KolorLiniiWykresu
            if(KolorLiniiWykresu.ShowDialog() == DialogResult.OK)
            {
                //zmiana koloru
                chtWykresSzeregu.Series[0].Color = KolorLiniiWykresu.Color;
                txtWziernikKolLin.BackColor = KolorLiniiWykresu.Color;

            }
        }

        private void txtKolotTła_TextChanged(object sender, EventArgs e)
        {
        }

        private void zmianaKoloruTłaWykresuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog KolorTła = new ColorDialog();

            //zaznaczenie  w KolorTła aktywny kolor
            KolorTła.Color = chtWykresSzeregu.BackColor;

            //wyświetlenie okna dialogowego KolorTła
            if (KolorTła.ShowDialog() == DialogResult.OK)
            {//zmiana koloru 
                chtWykresSzeregu.BackColor = KolorTła.Color;
                lblWykresZmian.BackColor = KolorTła.Color;
                txtKolotTła.BackColor = KolorTła.Color;
            }
        }

        private void cmbStylLinii_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbTypWykresu.SelectedIndex)
            {
                case 0:
                    chtWykresSzeregu.Series[0].ChartType = SeriesChartType.Line;
                    break;
                case 1:
                    chtWykresSzeregu.Series[0].ChartType = SeriesChartType.Column;
                    break;
                case 2:
                    chtWykresSzeregu.Series[0].ChartType = SeriesChartType.Bubble;
                    break;
                case 3:
                    chtWykresSzeregu.Series[0].ChartType = SeriesChartType.Point;
                    break;
                default:
                    chtWykresSzeregu.Series[0].ChartType = SeriesChartType.Line;
                    break;
            }
        }

        private void zmianaKoloruLiniiWykresuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //            chtWykresSzeregu.Series[0].Color = Color.Black;
            ColorDialog KolorLiniiWykresu = new ColorDialog();

            //zaznaczenie w KolorLiniiWykresu aktywny kolor
            KolorLiniiWykresu.Color = chtWykresSzeregu.Series[0].Color;

            //wyświetlenie okna dialogowego KolorLiniiWykresu
            if (KolorLiniiWykresu.ShowDialog() == DialogResult.OK)
            {
                //zmiana koloru
                chtWykresSzeregu.Series[0].Color = KolorLiniiWykresu.Color;
                txtWziernikKolLin.BackColor = KolorLiniiWykresu.Color;

            }
        }

        private void zmianaGrubościLiniiToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void zmianaKoloruTłaFormularzaToolStripMenuItem_Click(object sender, EventArgs e)
        {
              ColorDialog kolorTłaForm = new ColorDialog();

              kolorTłaForm.Color = this.BackColor;

              if(kolorTłaForm.ShowDialog() == DialogResult.OK)
              {

                this.BackColor = kolorTłaForm.Color;

              }    
        }

        private void zmianaKoloruCzcionkiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog kolorCzcionkiForm = new ColorDialog();

            kolorCzcionkiForm.Color = this.ForeColor;

            if (kolorCzcionkiForm.ShowDialog() == DialogResult.OK)
            {

                this.ForeColor = kolorCzcionkiForm.Color;

            }
        }

        private void zmianaStyluCzcionkiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog stylCzcionkiForm = new FontDialog();

            stylCzcionkiForm.Font = this.Font;

            if (stylCzcionkiForm.ShowDialog() == DialogResult.OK)
            {

                this.Font = stylCzcionkiForm.Font;

            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtDokładnośćObliczeńCalk.Text = "";
            txtDolnaGranicaCałk.Text = "";
            txtGornaGranicaCalk.Text = "";
            txtGrubośćLinii.Text = "1";
            txtObliczonaSumaSzeregu.Text = "";
            txtWartośćEps.Text = "";
            txtWartośćPrzyrostu.Text = "";
            txtWartośćX.Text = "";
            txtWartośćXd.Text = "";
            txtWartośćXg.Text = "";
            txtObliczonaWartCałki.Text = "";

            btnTabelarycznaWizualizacja.Enabled = true;
            btnGraficznaWizualizacja.Enabled = true;
            btnObliczSumęSzeregu.Enabled = true;
            btnKolorLinii.Enabled = false;
            btnKolorTlaObszWyk.Enabled = false;
            txtKolotTła.Enabled = false;
            txtWziernikKolLin.Enabled = false;
            cmbTypWykresu.Enabled = false;

            txtWartośćEps.Enabled = true;
            txtWartośćPrzyrostu.Enabled = true;
            txtWartośćXd.Enabled = true;
            txtWartośćXg.Enabled = true;

            dgvTabelaWartościSzeregu.Visible = false;
            chtWykresSzeregu.Visible = false;
            lblWykresZmian.Visible = false;

            odczytajTablicęWartościSzereguZPlikuToolStripMenuItem.Enabled = false;
            groupBox1.Enabled = false;



        }

        private void plikToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void lblGrubośćLiniiWyk_Click(object sender, EventArgs e)
        {

        }

        bool PobranieDanychDlaPotrzebCałkowania(out float rsEpsSzeregu, out float rsXdCałk,
            out float rsXgCałk, out float rsEpsCałk)
        {
            rsEpsSzeregu = rsXdCałk = rsXgCałk = rsEpsCałk = 0.0f;

            //pobranie danych z kontrolek formularza

            //rsEpsSzeregu( 0.0 < rsEpsSzeregu < 1.0 )
            //rsXdCałk, XgCalk ( rsXd < rsXg )
            //EpsCałkowania ( 0.0 < rsEps Całkowania < 0.05 )


            if (string.IsNullOrEmpty(txtWartośćEps.Text))
            {// "zapalenie" kontrolki errorProvider (sygnalizacja błędu)
                errorProvider1.SetError(txtWartośćEps,
                    "ERROR: musisz podać dokładności obliczeń rsEps!");

                return false;/*Zakończenie pobierania danych wejściowych i 
                                  zwrotne przekazanie wartości "false" */
            }
            else
                errorProvider1.Dispose(); // "zgaszenie" kontrolki errorProvider1

            if (!float.TryParse(txtWartośćEps.Text, out rsEpsSzeregu))
            {//sygnalizacja o błędach 
                errorProvider1.SetError(txtWartośćEps, "ERROR: wystąpił" +
                    " niedozwolony znak w zapisie wartość rsEps ");

                return false;
            }
            else
                errorProvider1.Dispose();//zgaszenie kontrolki errorProvider

            if ((rsEpsSzeregu <= 0.0F) || (rsEpsSzeregu >= 1.0F))
            {
                errorProvider1.SetError(txtWartośćEps, "ERROR: dokładności obliczeń" +
                   " rsEps musi spełniać warunek wejściowy: 0.0 < rsEps < 1.0");
                return false; /*zakonczenie pobierania danzch wejsciowych i
                               zwrotne przekazanie wartosci false*/
            }
            else
                errorProvider1.Dispose(); //zgaszenie kontrolki errorProvider1

            if (string.IsNullOrEmpty(txtWartośćXd.Text))
            {
                errorProvider1.SetError(txtWartośćXd, "ERROR: musisz" +
                    " podać wartość rsXd (dolnej granicy przedziału) ");
                return false; /* zakonczenie pobierania danych wejsciowych i
                               zwrotne przekazanie wartosci "false" */
            }
            else
                errorProvider1.Dispose();

            if (!float.TryParse(txtDolnaGranicaCałk.Text, out rsXdCałk))
            {
                errorProvider1.SetError(txtDolnaGranicaCałk, "ERROR: wystąpił" +
                    " niedozwolony znak w zapisie wartość rsXd ");
                return false; /* zakonczenie pobierania danych wejsciowych i
                               zwrotne przekazanie wartosci "false" */
            }
            else
                errorProvider1.Dispose();


            //pobranie górnej granicy przedzialu wartosci zmiennej rsX
            //sprawdzenie, czy zostala wpisana dolna granica przedzialu

            if (string.IsNullOrEmpty(txtGornaGranicaCalk.Text))
            {
                errorProvider1.SetError(txtGornaGranicaCalk, "ERROR: musisz" +
                    " podać wartość rsXd (dolnej granicy przedziału) ");
                return false; /* zakonczenie pobierania danych wejsciowych i
                               zwrotne przekazanie wartosci "false" */
            }
            else
                errorProvider1.Dispose();

            if (!float.TryParse(txtGornaGranicaCalk.Text, out rsXgCałk))
            {
                errorProvider1.SetError(txtWartośćXg, "ERROR: wystąpił" +
                    " niedozwolony znak w zapisie wartość rsXg ");
                return false; /* zakonczenie pobierania danych wejsciowych i
                               zwrotne przekazanie wartosci "false" */
            }
            else
                errorProvider1.Dispose();

            if (rsXdCałk > rsXgCałk)
            {
                errorProvider1.SetError(txtDolnaGranicaCałk, "ERROR: dolna granica przedziału" +
                    " nie może być większa od górnej granicy przedziału wartość rsXg");

                return false;
            }
            else
                errorProvider1.Dispose();


            if (string.IsNullOrEmpty(txtDokładnośćObliczeńCalk.Text))
            {// "zapalenie" kontrolki errorProvider (sygnalizacja błędu)
                errorProvider1.SetError(txtDokładnośćObliczeńCalk,
                    "ERROR: musisz podać dokładności obliczeń rsEps!");

                return false;/*Zakończenie pobierania danych wejściowych i 
                                  zwrotne przekazanie wartości "false" */
            }
            else
                errorProvider1.Dispose(); // "zgaszenie" kontrolki errorProvider1

            if (!float.TryParse(txtDokładnośćObliczeńCalk.Text, out rsEpsCałk))
            {//sygnalizacja o błędach 
                errorProvider1.SetError(txtDokładnośćObliczeńCalk, "ERROR: wystąpił" +
                    " niedozwolony znak w zapisie wartość rsEps ");

                return false;
            }
            else
                errorProvider1.Dispose();//zgaszenie kontrolki errorProvider

            if ((rsEpsCałk <= 0.0F) || (rsEpsCałk >= 0.05F))
            {
                errorProvider1.SetError(txtDokładnośćObliczeńCalk, "ERROR: dokładności obliczeń" +
                   " rsEps musi spełniać warunek wejściowy: 0.0 < rsEps < 0.05");
                return false; /*zakonczenie pobierania danzch wejsciowych i
                               zwrotne przekazanie wartosci false*/
            }
            else
                errorProvider1.Dispose(); //zgaszenie kontrolki errorProvider1



            return true;
        }

        static float SumaSzeregu(float rsX, float rsEps)
        {
            //deklaracja uzupewnienia

            float w, rsSumaWyrazów;
            int n;// rsLicznik wyrazów szeregu
            //ustalenie początkowego stanu obliczeń
            rsSumaWyrazów = 0.0F;
            w = 1.0F;
            n = 0;
            do
            {//obliczenie kolejnej  sumy czesciowej
                rsSumaWyrazów += w;
                n++;
                w *= (-1) * rsX / n;

        
            } while (Math.Abs(w)> rsEps);


            return rsSumaWyrazów;
        }


      /*   float ObliczanieCałkiMetodąProstokątów (float rsEpsSzeregu, 
            float rsXdCałk, float rsXgCałk, float rsEpsCałk, out float LicznikPrzedziałów, out float SzerokośćPrzedziału)
         {


            float rsH, Ci, Ci_1, SumaWartościSzeregu;
            float rsX;//wartość współrzędnej rsX śRODKóW przedziałów
            //ustalenie początkowego stanu obliczeń: pierwsze przybliżenie całki
            LicznikPrzedziałów = 1;
            //Wartości całki dla jednego przedziału (prostokaąta)
            Ci = (rsXgCałk - rsXdCałk) * SumaSzeregu((rsXdCałk + rsXgCałk) / 2.0F, rsEpsSzeregu);
            //iteracyjne obliczanie calki metoda prostokatow
            do
            {//przechowanie i tego prZYBLIZENIA W cI_1
                Ci_1 = Ci;
                //zwiększenie liczby podprzedziałów (prostokątów)
                LicznikPrzedziałów = LicznikPrzedziałów + LicznikPrzedziałów;
                //obliczenie szerokości podrzedziałów po zwiększeniu ich liczby
                rsH = (rsXgCałk - rsXdCałk) / LicznikPrzedziałów;
                //ustawienie wartości zmiennej rsX na środek pierwszego prostokąta
                rsX = rsXdCałk + rsH / 2.0F;

                SumaWartościSzeregu = 0.0F;
                for (ushort i = 0; i < LicznikPrzedziałów; i++)
                {
                    SumaWartościSzeregu += SumaSzeregu(rsX + i * rsH, rsEpsSzeregu);
                }

            } while (Math.Abs(Ci - Ci_1) > rsEpsCałk);
            //zwrotne przekazanie szerokości podprzedziłu
            SzerokośćPrzedziału = rsH;

            return Ci;
         }*/

        private void btnObliczCałkę_Click(object sender, EventArgs e)
        {
            float rsEpsSzeregu,
             rsXdCałk, rsXgCałk, rsEpsCałk;
             if (!PobranieDanychDlaPotrzebCałkowania(out rsEpsSzeregu, out rsXdCałk, out rsXgCałk, out rsEpsCałk))
                return;
            float rsH, Ci, Ci_1, SumaWartościSzeregu, rsX;
            float LicznikPrzedziałów;
            LicznikPrzedziałów = 1;
            //Wartości całki dla jednego przedziału (prostokaąta)
            Ci = (rsXgCałk - rsXdCałk) * SumaSzeregu((rsXdCałk + rsXgCałk) / 2.0F, rsEpsSzeregu);
            //iteracyjne obliczanie calki metoda prostokatow
            do
            {//przechowanie i tego prZYBLIZENIA W cI_1
                Ci_1 = Ci;
                //zwiększenie liczby podprzedziałów (prostokątów)
                LicznikPrzedziałów = LicznikPrzedziałów + LicznikPrzedziałów;
                //obliczenie szerokości podrzedziałów po zwiększeniu ich liczby
                rsH = (rsXgCałk - rsXdCałk) / LicznikPrzedziałów;
                //ustawienie wartości zmiennej rsX na środek pierwszego prostokąta
                rsX = rsXdCałk + rsH / 2.0F;

                SumaWartościSzeregu = 0.0F;
                for (ushort i = 0; i < LicznikPrzedziałów; i++)
                {
                    SumaWartościSzeregu += SumaSzeregu(rsX + i * rsH, rsEpsSzeregu);
                }

            } while (Math.Abs(Ci - Ci_1) > rsEpsCałk);
            //zwrotne przekazanie szerokości podprzedziłu
            float SzerokośćPrzedziału = rsH;

            txtObliczonaWartCałki.Text = string.Format("{0:F3}", Ci);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            int rsGrubośćLiniiMenu1 = 1;
            chtWykresSzeregu.Series[0].BorderWidth = rsGrubośćLiniiMenu1;
            //ustawienie w innych kontrolkach
            trbGrubośćLiniiWyk.Value = rsGrubośćLiniiMenu1;
            txtGrubośćLinii.Text = rsGrubośćLiniiMenu1.ToString();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            int rsGrubośćLiniiMenu4 = 4;
            chtWykresSzeregu.Series[0].BorderWidth = rsGrubośćLiniiMenu4;
            //ustawienie w innych kontrolkach
            trbGrubośćLiniiWyk.Value = rsGrubośćLiniiMenu4;
            txtGrubośćLinii.Text = rsGrubośćLiniiMenu4.ToString();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            int rsGrubośćLiniiMenu2 = 2;
            chtWykresSzeregu.Series[0].BorderWidth = rsGrubośćLiniiMenu2;
            //ustawienie w innych kontrolkach
            trbGrubośćLiniiWyk.Value = rsGrubośćLiniiMenu2;
            txtGrubośćLinii.Text = rsGrubośćLiniiMenu2.ToString();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            int rsGrubośćLiniiMenu3 = 3;
            chtWykresSzeregu.Series[0].BorderWidth = rsGrubośćLiniiMenu3;
            //ustawienie w innych kontrolkach
            trbGrubośćLiniiWyk.Value = rsGrubośćLiniiMenu3;
            txtGrubośćLinii.Text = rsGrubośćLiniiMenu3.ToString();
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            int rsGrubośćLiniiMenu5 = 5;
            chtWykresSzeregu.Series[0].BorderWidth = rsGrubośćLiniiMenu5;
            //ustawienie w innych kontrolkach
            trbGrubośćLiniiWyk.Value = rsGrubośćLiniiMenu5;
            txtGrubośćLinii.Text = rsGrubośćLiniiMenu5.ToString();
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            int rsGrubośćLiniiMenu6 = 6;
            chtWykresSzeregu.Series[0].BorderWidth = rsGrubośćLiniiMenu6;
            //ustawienie w innych kontrolkach
            trbGrubośćLiniiWyk.Value = rsGrubośćLiniiMenu6;
            txtGrubośćLinii.Text = rsGrubośćLiniiMenu6.ToString();
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            int rsGrubośćLiniiMenu7 = 7;
            chtWykresSzeregu.Series[0].BorderWidth = rsGrubośćLiniiMenu7;
            //ustawienie w innych kontrolkach
            trbGrubośćLiniiWyk.Value = rsGrubośćLiniiMenu7;
            txtGrubośćLinii.Text = rsGrubośćLiniiMenu7.ToString();
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            int rsGrubośćLiniiMenu8 = 8;
            chtWykresSzeregu.Series[0].BorderWidth = rsGrubośćLiniiMenu8;
            //ustawienie w innych kontrolkach
            trbGrubośćLiniiWyk.Value = rsGrubośćLiniiMenu8;
            txtGrubośćLinii.Text = rsGrubośćLiniiMenu8.ToString();
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            int rsGrubośćLiniiMenu9 = 9;
            chtWykresSzeregu.Series[0].BorderWidth = rsGrubośćLiniiMenu9;
            //ustawienie w innych kontrolkach
            trbGrubośćLiniiWyk.Value = rsGrubośćLiniiMenu9;
            txtGrubośćLinii.Text = rsGrubośćLiniiMenu9.ToString();
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            int rsGrubośćLiniiMenu10 = 10;
            chtWykresSzeregu.Series[0].BorderWidth = rsGrubośćLiniiMenu10;
            //ustawienie w innych kontrolkach
            trbGrubośćLiniiWyk.Value = rsGrubośćLiniiMenu10;
            txtGrubośćLinii.Text = rsGrubośćLiniiMenu10.ToString();
        }

        private void radBtnLiniiBezOpisu_CheckedChanged(object sender, EventArgs e)
        {
            
            chtWykresSzeregu.ChartAreas[0].AxisX.Title = "";
            chtWykresSzeregu.ChartAreas[0].AxisY.Title = "";

        }

        private void radBtnLiniiZOpisem_CheckedChanged(object sender, EventArgs e)
        {
            chtWykresSzeregu.ChartAreas[0].AxisX.Title = "Wartość zmiennej X";
            chtWykresSzeregu.ChartAreas[0].AxisY.Title = "Wartość szeregu S(X)";
        }

    }
}
