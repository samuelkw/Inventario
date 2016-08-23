using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Inventario
{
    class Program
    {
        private static DataTable dt;
        private static List<String> listaExt;
        private static List<int> listaQtd;
        static void Main(string[] args)
        {
            //Verifica se o usuário informou o caminho no (primeiro) parâmetro. Se não informou, sai da aplicação.
            if (args.Length != 1)
            {
                geraInfo("É preciso informar o caminho no primeiro parâmetro!", 1);
            }

            String path = args[0].ToString();
            DirectoryInfo dir = new DirectoryInfo(path);

            listaExt = new List<string> { };
            listaQtd = new List<int>{};

            dt = new DataTable();
            dt.Columns.Add("path", typeof(String));
            dt.Columns.Add("nome", typeof(String));
            dt.Columns.Add("ext", typeof(String));

            //Verifica se o caminho informado existe. Se não existir, sai da aplicação.
            if (!dir.Exists)
            {
                geraInfo("É preciso informar um caminho válido!", 2);
            }

            //Iniciar a chamada do método recursivo listaSubdir
            listaSubdir(dir);

            //Imprimir as extensões no console

            String appPath = System.Environment.CurrentDirectory;
            if (!appPath.EndsWith("\\")) appPath += "\\";
            String arqA = appPath + "InventarioAnalitico.csv";
            String arqS = appPath + "InventarioSintetico.csv";

            try
            {
                FileInfo flExt = new FileInfo(arqA);
                if (flExt.Exists) flExt.Delete();
            }
            catch
            {
                geraInfo("Não foi possível excluir o arquivo existente" + arqA, 2);
            }
            try
            {
                FileInfo flExt = new FileInfo(arqS);
                if (flExt.Exists) flExt.Delete();
            }
            catch
            {
                geraInfo("Não foi possível excluir o arquivo existente" + arqS, 2);
            }


            StreamWriter fA = new StreamWriter(arqA, true);
            StreamWriter fS = new StreamWriter(arqS, true);
            fA.AutoFlush = true;
            fS.AutoFlush = true;

            fA.WriteLine("path;nome;ext");
            fS.WriteLine("ext;qtde");
            foreach (DataRow r in dt.Rows)
            {
                fA.WriteLine(r[0].ToString() + ";" + r[1].ToString() + ";" + r[2].ToString());
                bool novoExt = true;
                for (int i = 0; i < listaExt.Count; i++ )
                {
                    String ext = listaExt[i];
                    if (ext.Equals(r[2].ToString()))
                    {
                        novoExt = false;
                        listaExt[i] = listaExt[i];
                        listaQtd[i] = listaQtd[i] + 1;
                    }
                }
                if (novoExt)
                {
                    listaExt.Add(r[2].ToString());
                    listaQtd.Add(1);
                }
            }

            for (int i = 0; i < listaExt.Count; i++)
            {
                fS.WriteLine(listaExt[i].ToString() + ";" + listaQtd[i].ToString());
            }
            fA.Close();
            fS.Close();

            //Saindo...
            geraInfo("Adeus!", 0);

        }


        public static void listaSubdir(DirectoryInfo dir)
        {
            //Vamos listar os arquivos do diretório informado
            foreach (FileInfo arq in dir.GetFiles())
            {
                String path = arq.DirectoryName;
                String nome = arq.Name;
                String ext = arq.Extension;
                if (!path.EndsWith("\\"))
                {
                    path += "\\";
                }
                if (nome.LastIndexOf(".") > 0)
                {
                    nome = nome.Substring(0, nome.LastIndexOf("."));
                }
                if (ext.StartsWith("."))
                {
                    ext=ext.Substring(1);
                }
                dt.Rows.Add(path,nome,ext);
            }
            //Vamos listar os possíveis subdiretórios, e recursivamente varrer seus arquivos e seus subdiretórios
            foreach (DirectoryInfo subdir in dir.GetDirectories())
            {
                listaSubdir(subdir);
            }
        }

        public static void geraInfo(String msg, int opt)
        {
            DateTime dt = DateTime.Now;
            System.Console.WriteLine(dt.ToString("dd/MM/yyyy HH:mm:ss") + ": " + msg);
            if (opt >= 0)
            {
                System.Environment.Exit(opt);
            }
        }
    }
}
