using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Data.Context;
using Api.Domain.Entities;
using Api.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Repository
{
    public class BaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        //Injeção de dependencia
        protected readonly MyContext _context;
        private DbSet<T> _dataset;
        public BaseRepository(MyContext context)
        {
            _context = context;
            _dataset = _context.Set<T>();
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                //Fazer um select do registro que já existe na base.
                var result = await _dataset.SingleOrDefaultAsync(p => p.Id.Equals(id));

                //verificar se o que foi encontrado na base é nulo.
                if (result == null)
                {
                    return false; //não pode retornar nulo.
                }

                //tendo encontrado registro no result ele vai apagar no BD.
                _dataset.Remove(result);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<T> InsertAsync(T item)
        {
            try
            {
                //analisa se o id está vazio, se estiver vazio ele atribui um guid.
                if (item.Id == Guid.Empty)
                {
                    item.Id = Guid.NewGuid();
                }

                //recebe a data atual utc
                item.CreateAt = DateTime.UtcNow;

                //o dataset recebe o item que foi passado.
                _dataset.Add(item);

                //salvamos o context. Vai salvar no nosso banco de dados.  
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return item;
        }

        public async Task<bool> ExistAsync(Guid id)
        {
            return await _dataset.AnyAsync(p => p.Id.Equals(id));
        }

        public async Task<T> SelectAsync(Guid id)
        {
            try
            {
                return await _dataset.SingleOrDefaultAsync(p => p.Id.Equals(id));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<IEnumerable<T>> SelectAysnc()
        {
            try
            {
                return await _dataset.ToListAsync();
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public async Task<T> UpdateAsync(T item)
        {
            try
            {
                //Fazer um select do registro que já existe na base.
                var result = await _dataset.SingleOrDefaultAsync(p => p.Id.Equals(item.Id));

                //verificar se o que foi encontrado na base é nulo.
                if (result == null)
                {
                    return null;
                }

                item.UpdateAt = DateTime.UtcNow;

                //força o createat sempre pegar a data do nosso result.
                item.CreateAt = result.CreateAt;

                //vai pegar o result e setar os valores e salvar no banco de dados.
                _context.Entry(result).CurrentValues.SetValues(item);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return item;
        }
    }
}
