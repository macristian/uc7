using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace Biblioteca.Models
{
    public class EmprestimoService
    {
        public void Inserir(Emprestimo e)
        {
            using (BibliotecaContext bc = new BibliotecaContext())
            {
                bc.Emprestimos.Add(e);
                bc.SaveChanges();
            }
        }

        public void Atualizar(Emprestimo e)
        {
            using (BibliotecaContext bc = new BibliotecaContext())
            {
                Emprestimo emprestimo = bc.Emprestimos.Find(e.Id);
                emprestimo.NomeUsuario = e.NomeUsuario;
                emprestimo.Telefone = e.Telefone;
                emprestimo.LivroId = e.LivroId;
                emprestimo.DataEmprestimo = e.DataEmprestimo;
                emprestimo.DataDevolucao = e.DataDevolucao;
                emprestimo.Devolvido = e.Devolvido;

                bc.SaveChanges();
            }
        }

        public ICollection<Emprestimo> ListarTodos(FiltrosEmprestimos filtro)
        {
            using (BibliotecaContext bc = new BibliotecaContext())
            {
                IQueryable<Emprestimo> consulta;

                if (filtro != null)
                {
                    //definindo dinamicamente a filtragem
                    switch (filtro.TipoFiltro)
                    {
                        case "Usuario":
                            //consulta = bc.Emprestimos.Where(e => e.NomeUsuario.Contains(filtro.Filtro));
                            consulta = bc.Emprestimos.Where(e => e.NomeUsuario.Contains(filtro.Filtro));
                            break;

                        case "Livro":
                            //consulta = bc.Emprestimos.Where(e => e.Livro.Titulo.Contains(filtro.Filtro));
                            List<Livro> LivrosFiltrados = bc.Livros.Where(l => l.Titulo.Contains(filtro.Filtro)).ToList();

                            List<int>LivrosIds = new List<int>();
                            for (int i = 0; i < LivrosFiltrados.Count; i++)
                            {LivrosIds.Add(LivrosFiltrados[i].Id);}

                            consulta = bc.Emprestimos.Where(e => LivrosIds.Contains(e.LivroId));
                            var debug = consulta.ToList();
                            break;

                        default:
                            consulta = bc.Emprestimos;
                            break;
                    }
                }
                else
                {
                    // caso filtro não tenha sido informado
                    consulta = bc.Emprestimos;
                }
                List<Emprestimo>ListaConsulta = consulta.OrderBy(e => e.DataEmprestimo).ToList();
                for (int i = 0; i < ListaConsulta.Count; i++)
                {
                    ListaConsulta[i].Livro = bc.Livros.Find(ListaConsulta[i].LivroId);
                }
                return ListaConsulta;

                //return consulta.OrderBy(e => e.NomeUsuario).ToList();
                //return bc.Emprestimos.Include(e => e.Livro).ToList();
            }
        }

        public Emprestimo ObterPorId(int id)
        {
            using (BibliotecaContext bc = new BibliotecaContext())
            {
                return bc.Emprestimos.Find(id);
            }
        }
    }
}