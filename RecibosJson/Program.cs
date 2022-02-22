using System;
using Newtonsoft.Json;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RecibosJson
{
    //Correo: joaquin@pecano.pe
    enum TipoComprobante {Factura=10 , Boleta=20 ,NotaCredito=30, NotaDebito=40 };

    class Comprobante
    {
        public string NumeroDocumento;
        public DateTime FechaEmision;
        public int TipoDocumento;
        public int Estado;
        public string DocumentoIdentidadAdquiriente;
        public string RazonSocialAdquiriente;
        public double ImporteTotal;

        public String GetDescription()
        {
            TipoComprobante t = (TipoComprobante)TipoDocumento;
            switch (t)
            {
                case TipoComprobante.Factura:
                    return "FACTURA";
                case TipoComprobante.Boleta:
                    return "BOLETA";
                case TipoComprobante.NotaCredito:
                    return "NOTA DE CREDITO";
                case TipoComprobante.NotaDebito:
                    return "NOTA DE DEBITO";
                default:
                    break;
            }
            return "DESC";
        }

        //ya no se usó :C
        public string serializar()
        {
            return String.Format("{0}|{1}|{2}",TipoDocumento.ToString(), ((TipoComprobante)TipoDocumento).ToString(),FechaEmision.ToString(), ImporteTotal);
        }
    }

    class Program
    {
        
        static void Main(string[] args)
        {
            FileStream fstream = File.OpenRead("Comprobantes.json");

            string data = File.ReadAllText("Comprobantes.json");
            
            List<Comprobante> comprobantes = JsonConvert.DeserializeObject<List<Comprobante>>(data);

           
            while (true)
            {
                Console.WriteLine("Para Generar informe agrupado por documentos presione 1");
                Console.WriteLine("Para Seleccionar por fecha presione 2");
                Console.WriteLine("Para salir presione R");
                string res = Console.ReadLine();
                List<Comprobante> resultado;
                switch (res)
                {
                    case "1":
                        GroupByTypes(comprobantes);
                        
                        
                        break;
                        
                    case "2":
                        Console.WriteLine("Ingrese fecha");
                        string fecha = Console.ReadLine();
                        try
                        {
                            groupByDate(comprobantes, DateTime.Parse(fecha));
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        break;

                    case "R":
                        return;
                        break;
                    default:
                        break;
                }
            }

            Console.WriteLine(comprobantes.Count);
        }

        static String GetHeader()
        { 
            return String.Format("{0}|{1}|{2}|{3}{4}", "TIPO DOCUMENTO", "DESCRIPCION", "FECHA EMISION", "CANTIDAD CPE","\n");
        }

        static void groupByDate(List<Comprobante> _comprobantes , DateTime _fecha)
        {
            IEnumerable<Comprobante>  res = _comprobantes.Where(s => s.FechaEmision >= _fecha && s.FechaEmision <= _fecha)
                              .Select(s => s);
            String data = GetHeader();
            Console.WriteLine("generando informe agrupados por fecha ingresada " + _fecha.ToString());
            foreach (Comprobante item in res)
            {
                data += String.Format("{0}|{1}|{2}|{3}{4}", item.TipoDocumento, item.GetDescription() ,item.FechaEmision , res.Count() ,"\n" );
            }

            File.WriteAllTextAsync("InformePorFechas.txt", data);
            Console.WriteLine("informe generado en : InformePorFechas.txt");
            Console.ReadLine();//esperar
            Console.Clear();

        }

        /*
        static List<Comprobante> GroupByType(List<Comprobante> _comprobantes , int _tipo)
        {
            IEnumerable<Comprobante> res = _comprobantes.Where(s => s.TipoDocumento == _tipo)
                              .Select(s => s );
            return res.ToList();
        }*/

        static void GroupByTypes(List<Comprobante> _comprobantes)
        {
            IEnumerable<Comprobante> factura = _comprobantes.Where(s => s.TipoDocumento == 10)
                              .Select(s => s); //factura
            IEnumerable<Comprobante> boleta = _comprobantes.Where(s => s.TipoDocumento == 20)
                             .Select(s => s); //boleta
            IEnumerable<Comprobante> notaCred = _comprobantes.Where(s => s.TipoDocumento == 30)
                             .Select(s => s); //222
            IEnumerable<Comprobante> notaDeb = _comprobantes.Where(s => s.TipoDocumento == 40)
                             .Select(s => s); //40

            //generar ....
            factura.ToList<Comprobante>();
            boleta.ToList<Comprobante>();
            notaCred.ToList<Comprobante>();
            notaDeb.ToList<Comprobante>();


            String data = GetHeader();

            data += String.Format("{0}|{1}|{2}|{3}","FACTURA" , factura.FirstOrDefault().FechaEmision.ToString() , factura.Count(), "\n");
            data += String.Format("{0}|{1}|{2}|{3}", "BOLETA", boleta.FirstOrDefault().FechaEmision.ToString(), boleta.Count(), "\n") ;
            data += String.Format("{0}|{1}|{2}|{3}", "NOTA CREDITO", notaCred.FirstOrDefault().FechaEmision.ToString(), notaCred.Count(), "\n");
            data += String.Format("{0}|{1}|{2}|{3}", "NOTA DEBITO", notaDeb.FirstOrDefault().FechaEmision.ToString(), notaDeb.Count() , "\n");

            File.WriteAllTextAsync("InformePorTipoDocumento.txt", data);
            Console.WriteLine("informe generado en : InformePorTipoDocumento.txt");
            Console.ReadLine();//esperar
            Console.Clear();
        }

    }
}
