from openai_api.client import Client as OpenAIClient
from openai_api.schema import OpenAIRequestModel, OpenAIModel
from bff_interaction.client import Client as DataClient
from agent.schema import *
from config import OPENAI_TOKEN_VAR, DEEPSEEK_TOKEN_VAR, DEEPSEEK_URL_VAR

import os


class AIAgentService:
    def __init__(self):
        self.openai_client = OpenAIClient(api_key=os.getenv(OPENAI_TOKEN_VAR))
        self.data_client = DataClient()
    
    async def request(self, agent_request: Optional[AgentRequestModel], user_message: Optional[str]) -> AgentResponseModel:
        if user_message is None and agent_request is None:
            raise ValueError("user_message and messages cannot be None at the same time!")
        
        if agent_request is None: 
            messages = self.data_client.get_context()
            # добавляем сообщение пользователя и информацию о его группе обучения
            # messages.add(MessageModel(role=OpenAIRole.USER, content=self._get_user_data_str(group, university)))
        else:
            messages = agent_request.messages
            # messages._update_date()
        
        if user_message is not None:
            messages.add(MessageModel(role=OpenAIRole.USER, content=user_message))

        tools = self.data_client.get_tools()

        request_model = OpenAIRequestModel(
        model=OpenAIModel.GPT_4O,
        messages=messages,
        tools=tools
        )

        result = await self.openai_client.request(request_model)
        
        # сохраняем в messages информацию, которую нам предоставил gpt о том, какие функции нужно вызвать 
        if result.choices[0].message.tool_calls is not None:
            tool_calls = ToolCalls.vallidate_from_gpt_resp(result.choices[0].message.tool_calls)
            messages.add(ToolCallsHistory(tool_calls=tool_calls.tool_calls, content=result.choices[0].message.content))
        else:
            messages.add(ToolCallsHistory(tool_calls=None, content=result.choices[0].message.content))

        return AgentResponseModel(messages=messages)
    
    # def _get_user_data_str(self, group: str, university: str) -> str:
    #     return f"Университет: {university}, группа: {group}"
    

ai_agent = AIAgentService()
