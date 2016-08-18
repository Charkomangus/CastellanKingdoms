using UnityEngine;
//using System.Collections;
//using System.Xml;
//using System.Xml.Serialization; 
//using System.IO; 
//using System; 
//
//public class LoadPoliticalCard : MonoBehaviour 
//{
//	// Use this for initialization
//	public void Start () 
//	{
//		//instantiates new xml document and loads data
//		XmlDocument politcalEvents = new XmlDocument (); 
//		politcalEvents.Load ("PoliticalEvents"); 
//
//		ProcessEvents(politcalEvents.SelectNodes("PoliticalEvents/Event"));
//	}
//
//	private void ProcessEvents(XmlNodeList nodes)
//	{
//        CardPolitics m_politcalEvent; 
//
//		foreach (XmlNode node in nodes)
//		{
//			m_politcalEvent = new CardPolitics(); 
//			m_politcalEvent.title = Convert.ToInt16(node.Attributes.GetNamedItem("title").Value); 
//			m_politcalEvent.description = node.SelectSingleNode("Title").InnerText; 
//			m_politcalEvent.choices = node.SelectSingleNode("Description").InnerText; 
//			
//			LoadEvent (m_politcalEvent); 
//		}
//	}
//
//	private void LoadEvent(CardPolitics Political)
//	{
//		GameObject pol_cardGameObject = GameObject.Find ("CardPolitics" + Political.title.ToString ());
//		pol_cardGameObject.SendMessage ("LoadEvent", Political); 
//	}
//}
