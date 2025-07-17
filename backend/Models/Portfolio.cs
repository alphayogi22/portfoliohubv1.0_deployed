using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PortfolioApi.Models
{
    public class Portfolio
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("image")]
        public byte[] Image { get; set; } = Array.Empty<byte>();

        [BsonElement("imageContentType")]
        public string ImageContentType { get; set; } = string.Empty;

        [BsonElement("resume")]
        public byte[] Resume { get; set; } = Array.Empty<byte>();

        [BsonElement("resumeContentType")]
        public string ResumeContentType { get; set; } = string.Empty;

        [BsonElement("usernameKey")]
        public string UsernameKey { get; set; } = string.Empty;
    }
}