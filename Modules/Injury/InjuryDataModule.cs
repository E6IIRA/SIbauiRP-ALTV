using System.Linq;
using GangRP_Server.Core;
using GangRP_Server.Models;
using Microsoft.EntityFrameworkCore;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Modules.Injury
{
    public sealed class InjuryDataModule : DataModuleBase<InjuryDataModule, InjuryTypeData>
    {
        internal override int GetId(InjuryTypeData item) => item.Id;
        internal override IQueryable GetData(DbSet<InjuryTypeData> dbSet) => dbSet.Include(d => d.InjuryDeathCauseData);

        //internal override void OnItemLoaded(InjuryTypeData item)
        //{
        //    AltV.Net.Alt.Log("Injurytype loaded " + item.Name);
        //}
    }
}
