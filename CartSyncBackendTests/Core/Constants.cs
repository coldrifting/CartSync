namespace CartSyncBackendTests.Core;

public static class Constants
{
    public static int BadRequestStatusCode => 400;
    public static int NotFoundStatusCode => 404;

    public static Ulid BadId => Ulid.Parse("00000000000000000000000000");
    public static Ulid NotFoundId => Ulid.Parse("40400000000000000000000404");
    public static string BadIdString => "badId";
}