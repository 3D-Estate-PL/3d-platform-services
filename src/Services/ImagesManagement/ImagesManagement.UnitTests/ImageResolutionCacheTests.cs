using ImagesManagement.Domain;

namespace ImagesManagement.UnitTests;

public class ImageResolutionCacheTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void image_resolution_with_the_same_properties_should_be_equal()
    {
        // Arrange
        var imageResolution1 = new ImageResolutionEntity
        {
            Width = 100,
            Height = 100,
            Compression = 90
        };
        
        var imageResolution2 = new ImageResolutionEntity
        {
            Width = 100,
            Height = 100,
            Compression = 90
        };
        
        // Assert
        Assert.IsTrue(imageResolution1 == imageResolution2);
        Assert.IsTrue(imageResolution1.Equals(imageResolution2));
    }

    [Test]
    public void image_resolution_can_be_add_only_once()
    {
        // Arrange
        var image = new ImageMetadataAggregate("product", "28F62E0D-BFB1-464C-A844-6E9B483F9636",DateTimeOffset.Now);

        // Act
        var imageResolution = new ImageResolutionEntity
        {
            Width = 100,
            Height = 100,
            Compression = 90
        };
        image.AddResolution(imageResolution);
        image.AddResolution(imageResolution);
        
        // Assert
        Assert.IsTrue(image.HasResolution(imageResolution));
        Assert.That(image.Resolutions.Count, Is.EqualTo(1));
    }
    
    [Test]
    public void has_resolution_method_should_return_true_if_resolution_exists()
    {
        // Arrange
        var image = new ImageMetadataAggregate("product", "28F62E0D-BFB1-464C-A844-6E9B483F9636", DateTimeOffset.Now);
        
        // Act
        var imageResolution = new ImageResolutionEntity
        {
            Width = 100,
            Height = 100,
            Compression = 90
        };
        image.AddResolution(imageResolution);
        
        // Assert
        Assert.That(image.HasResolution(new ImageResolutionEntity
        {
            Width = 100,
            Compression = 90,
            Height = 100,
        }), Is.True);
    }
    
    [Test]
    public void has_resolution_method_should_return_false_if_resolution_not_exists()
    {
        // Arrange
        var image = new ImageMetadataAggregate("product", "28F62E0D-BFB1-464C-A844-6E9B483F9636",DateTimeOffset.Now);
        
        // Act
        image.AddResolution(new ImageResolutionEntity
        {
            Width = 100,
            Height = 100,
            Compression = 90
        });
        
        // Assert
        Assert.That(image.HasResolution(new ImageResolutionEntity
        {
            Width = 10,
            Height = 10,
            Compression = 90
        }), Is.False);
    }
}