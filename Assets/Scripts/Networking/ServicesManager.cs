using System.Threading.Tasks;
using Unity.Services.Core;

public class ServicesManager 
{
	public async void Initialize(LocalLobby i_locallobby, LocalLobbyUser i_localLobbyUser, UpdateRunner i_updateRunner)
	{
#if !DEDICATED_SERVER
		m_lobbyServiceFacade = new LobbyServiceFacade(i_locallobby, i_localLobbyUser, i_updateRunner);

		m_authServiceFacade = new AuthServiceFacade(i_localLobbyUser);
		await m_authServiceFacade.InitializeAndSignInAsync(null);
#endif

		m_multiplayServiceFacade = new MultiplayServiceFacade();
		await m_multiplayServiceFacade.Initialize();
	}

	public void Update()
	{
		m_multiplayServiceFacade.Update();
	}

	public LobbyServiceFacade m_lobbyServiceFacade { private set; get; }
	public AuthServiceFacade m_authServiceFacade { private set; get; }
	public MultiplayServiceFacade m_multiplayServiceFacade {  private set; get; }
}
