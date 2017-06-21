using Domain;

namespace Core.Domain
{
    public class Player : DomainObject
    {

        public static Player Default()
        {
            return new Player();
        }
    }
}
