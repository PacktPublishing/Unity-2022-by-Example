using UnityEngine;
using System;

public class KeyItemFactory : IDisposable
{
    private bool _disposedValue;

    public KeyItem_Factory CreateKeyItem(KeyItemData keyData, Vector3 position, Quaternion rotation)
    {
        var key = new GameObject($"Key_{keyData.Name}");
        key.transform.SetPositionAndRotation(position, rotation);

        var renderer = key.AddComponent<SpriteRenderer>();
        renderer.sprite = keyData.Sprite;
        renderer.color = keyData.Color;

        var collider = key.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;

        var item = key.AddComponent<KeyItem_Factory>();
        item.Init(keyData.Id);

        return item;
    }


    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~KeyItemFactory()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
