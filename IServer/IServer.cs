using System;
using System.Collections.Generic;
using System.Text;

namespace PractiSES
{
    public interface IServer
    {
        String InitKeySet_AskQuestions(String userID, String email);
        bool InitKeySet_EnvelopeAnswers(String userID, String email, String answersEnveloped);
        bool InitKeySet_SendPublicKey(String userID, String email, String publicKey, String macValue);
        String KeyObt(String email);
        String KeyObt(String email, DateTime date);
        bool KeyRem(String userID, String email, String signedMessage);
        String USKeyRem_AskQuestions(String userID, String email);
        void USKeyRem_EnvelopeAnswers(String userID, String email, String answersEnveloped);
        bool USKeyRem_SendRemoveRequest(String userID, String email, String macValue);
        String USKeyUpdate_AskQuestions(String userID, String email);
        void USKeyUpdate_EnvelopeAnswers(String userID, String email, String answersEnveloped);
        bool USKeyUpdate_SendPublicKey(String userID, String email, String publicKey, String macValue);
    }
}
