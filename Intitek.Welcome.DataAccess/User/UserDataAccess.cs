using System;
using System.Collections.Generic;
using System.Linq;
using Intitek.Welcome.Domain;
using Intitek.Welcome.Repository;
using Intitek.Welcome.Infrastructure.UnitOfWork;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace Intitek.Welcome.DataAccess
{
    public class UserDataAccess : Repository<IntitekUser, int>, IUserDataAccess
    {
        public UserDataAccess(IUnitOfWork uow) : base(uow)
        {

        }

        public new string Save(IntitekUser user)
        {
            var exceptionMessage = string.Empty;
            try
            {
                base.Save(user);
                return exceptionMessage;
            }
            catch (DbEntityValidationException ex)
            {
                //exceptionMessage = string.Format("{0}{1}{2}", Environment.CommandLine, ex.InnerException.Message, ex.EntityValidationErrors.ToString());
                exceptionMessage = string.Format("{0}{1}{2}{3}{4}", Environment.CommandLine, ex.InnerException.Message, ex.EntityValidationErrors.ToString(), ex.InnerException.InnerException.Message, ex.ToString());
            }
            catch (DbUpdateException ex)
            {
                //exceptionMessage = string.Format("{0}{1}{2}", Environment.CommandLine, ex.InnerException.Message, ex.Entries.ToString());
                exceptionMessage = string.Format("{0}{1}{2}{3}{4}", Environment.CommandLine, ex.InnerException.Message, ex.Entries.ToString(), ex.InnerException.InnerException.Message, ex.ToString());
            }

            catch (InvalidOperationException ex)
            {
                //exceptionMessage = string.Format("{0}{1}", Environment.CommandLine, ex.InnerException.Message);
                exceptionMessage = string.Format("{0}{1}{2}{3}{4}", Environment.CommandLine, ex.InnerException.Message, ex.Message, ex.InnerException.InnerException.Message, ex.ToString());
            }
            catch (Exception ex)
            {
                //exceptionMessage = ex.InnerException.Message;
                exceptionMessage = string.Format("{0}{1}{2}{3}{4}", Environment.CommandLine, ex.InnerException.Message, ex.StackTrace, ex.InnerException.InnerException.Message, ex.ToString());
            }
            finally
            {
                SetEntityState(user, EntityState.Detached);
               
            }
            return exceptionMessage;
        }

        public new string Add(IntitekUser user)
        {
            var exceptionMessage = string.Empty;

            try
            {
                base.Add(user);
                return string.Empty;
            }
            catch (DbEntityValidationException ex)
            {
                //exceptionMessage = string.Format("{0}{1}{2}", Environment.CommandLine, ex.InnerException.Message, ex.EntityValidationErrors.ToString());
                exceptionMessage = string.Format("{0}{1}{2}{3}{4}", Environment.CommandLine, ex.InnerException.Message, ex.EntityValidationErrors.ToString(), ex.InnerException.InnerException.Message, ex.ToString());
            }
            catch (DbUpdateException ex)
            {
                //exceptionMessage = string.Format("{0}{1}{2}", Environment.CommandLine, ex.InnerException.Message, ex.InnerException.InnerException.Message);
                exceptionMessage = string.Format("{0}{1}{2}{3}{4}", Environment.CommandLine, ex.InnerException.Message, ex.Entries.ToString(), ex.InnerException.InnerException.Message, ex.ToString());
            }

            catch (InvalidOperationException ex)
            {
                //exceptionMessage = string.Format("{0}{1}", Environment.CommandLine, ex.InnerException.Message);
                exceptionMessage = string.Format("{0}{1}{2}{3}{4}", Environment.CommandLine, ex.InnerException.Message, ex.Message, ex.InnerException.InnerException.Message, ex.ToString());
            }
            catch (Exception ex)
            {
                //exceptionMessage = ex.InnerException.Message;
                exceptionMessage = string.Format("{0}{1}{2}{3}{4}", Environment.CommandLine, ex.InnerException.Message, ex.Message, ex.InnerException.InnerException.Message, ex.ToString());
            }
            finally
            {
                SetEntityState(user, EntityState.Detached);

            }
            return exceptionMessage;
        }

        public void Update(IntitekUser user)
        {
            MergeEntityToContext(user);
            UnitOfWork.SaveChanges();
        }

        private void MergeEntityToContext(IntitekUser user)
        {

            var userToUpdate = base.Context.IntitekUser.Include("ProfileUser").SingleOrDefault(p => p.ID == user.ID);

            if (userToUpdate != null)
            {
                userToUpdate.Id = userToUpdate.ID;

                List<ProfileUser> lProfileToDelete = new List<ProfileUser>();
                foreach (ProfileUser c in userToUpdate.ProfileUser)
                {
                    c.Id = c.ID;
                    if (!user.ProfileUser.Where(x => x.ID_Profile == c.ID_Profile).Any())
                    {
                        lProfileToDelete.Add(c);
                    }
                }
                foreach (ProfileUser c in lProfileToDelete)
                {
                    userToUpdate.ProfileUser.Remove(c);
                    this.Context.Entry<ProfileUser>(c).State = EntityState.Deleted;
                }
                foreach (ProfileUser pu in user.ProfileUser)
                {
                    //Comparaison SOurce du context par rapport à l'objet envoyé                
                    if (!userToUpdate.ProfileUser.Where(x => x.ID_Profile == pu.ID_Profile).Any())
                    {
                        pu.ID_IntitekUser = user.ID;
                        this.Context.Entry<ProfileUser>(pu).State = EntityState.Added;
                    }
                }
            }

           
        }
        public void RemoveAllInactivity()
        {
            this.Context.Database.ExecuteSqlCommand( "UPDATE dbo.IntitekUser SET InactivityStart=NULL, InactivityEnd=NULL, InactivityReason=NULL WHERE InactivityStart is not null");
        }
        
       
    }
}
