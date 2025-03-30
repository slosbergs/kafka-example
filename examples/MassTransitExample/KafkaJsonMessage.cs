namespace MassTransitExample;

public class KafkaJsonMessage
{
    public string Schema { get; set; }
    public int Payload { get; set; }
}

public class InvMessage
{
    public StatusModel Status { get; set; }
    public ResultModel Result { get; set; }
    //public string Schema { get; set; }
    //public string Payload { get; set; }
}

public class StatusModel
{
    public string Code { get; set; }
    public string TraceId { get; set; }
}

public class ResultModel
{
    public string CallSeq { get; set; }
}

