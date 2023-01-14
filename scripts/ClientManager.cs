using Godot;
using System;

// Code executed on the client side only, handles network events
public partial class ClientManager : Node
{
    [Export] private string _address = "localhost";
    [Export] private int _port = 9999;

    private SceneMultiplayer _sceneMultiplayer = new();

    public override void _Ready()
    {
        Connect();
    }

    private void OnPeerConnected(long id) { }

    private void OnConnectedToServer()
    {
        GetNode<Label>("Debug/Label").Text += $"\n{Multiplayer.GetUniqueId()}";
    }

    private void Connect()
    {
        _sceneMultiplayer.PeerConnected += OnPeerConnected;
        _sceneMultiplayer.ConnectedToServer += OnConnectedToServer;
        _sceneMultiplayer.PeerPacket += OnPacketReceived;

        ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
        peer.CreateClient(_address, _port);
        _sceneMultiplayer.MultiplayerPeer = peer;
        GetTree().SetMultiplayer(_sceneMultiplayer);
        GD.Print("Client connected to ", _address, ":", _port);
    }

    private void OnPacketReceived(long id, byte[] data)
    {
        var state = StructHelper.ToStructure<GameState>(data);

        int senderId = state.Id;
        Vector3 position = new Vector3(state.X, state.Y, state.Z);

        GetNode<Node3D>("/root/Main/CharacterArray/" + senderId.ToString()).Position = position;
    }
}