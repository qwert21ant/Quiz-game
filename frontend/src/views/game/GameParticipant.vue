<template>
  <div class="ma-auto" v-if="!isInitialized">
    <v-progress-circular indeterminate size="x-large"/>
  </div>
  <div v-else>
    <v-container max-width="1000px">
      <v-card>
        <v-card-title class="d-flex">
          <div>Комната {{ roomInfo.name }}</div>
          <v-btn
            class="ml-auto"
            color="error"
            variant="outlined"
            prepend-icon="mdi-close"
            text="Покинуть"
            @click="leaveRoom"
          />
        </v-card-title>
        <v-card-text>
          {{ roomInfo.description ?? "Без описания" }}
        </v-card-text>
      </v-card>
    </v-container>
    <v-container
      v-if="!isGameRunning"
      class="d-flex flex-column align-center"
    >
      <v-progress-circular indeterminate size="x-large"/>
      <div class="mt-3">Ожидание других участников</div>
    </v-container>
    <div v-else>
      <v-container
        v-if="gameState.type == GameStateType.Answer"
        max-width="1000px"
      >
        <v-card>
          <v-card-title>Правильный ответ: {{ gameState.answer.answer }}</v-card-title>
          <v-card-text v-if="answer">
            Ваш ответ: <span class="text-waight-bold">{{ answer }}</span>
          </v-card-text>
        </v-card>
      </v-container>
      <v-container
        v-if="gameState.type == GameStateType.Waiting || gameState.type == GameStateType.Answer"
        max-width="1000px"
        class="d-flex flex-column align-center"
      >
        <v-progress-circular indeterminate size="x-large"/>
        <div class="mt-3">Ожидание следующего вопроса</div>
      </v-container>
      <v-container
        v-if="gameState.type == GameStateType.Question"
        max-width="1000px"
      >
        <v-card>
          <v-card-title>Вопрос: {{ gameState.question.text }}</v-card-title>
          <v-card-text class="pb-0">
            <v-text-field
              v-model="answer"
              v-if="gameState.question.type == QuizQuestionType.Text"
              label="Ответ"
              :disabled="answered"
            />
            <v-select
              v-model="answerOptionInd"
              v-if="gameState.question.type == QuizQuestionType.Choise"
              label="Ответ"
              :disabled="answered"
              :items="questionOptions"
            /> <!-- todo: show cards -->
          </v-card-text>
          <v-card-actions>
            <v-spacer />
            <v-btn
              variant="elevated"
              text="Ответить"
              :disabled="!canAnswer && !answered"
              @click="doAnswer"
            />
          </v-card-actions>
        </v-card>
      </v-container>
      <v-container
        v-if="gameState.type == GameStateType.Results"
        max-width="1000px"
      >
        <v-card>
          <v-card-title>
            Ваше место: <span class="text-weight-bold">{{ gameState.results.place }}</span>
          </v-card-title>
        </v-card>
      </v-container>
    </div>
  </div>
</template>

<script lang="ts">
import GameState from '@/models/game/GameState';
import GameStateType from '@/models/game/GameStateType';
import QuizQuestionType from '@/models/quiz/QuizQuestionType';
import RoomInfo from '@/models/room/RoomInfo';
import router from '@/router';
import GameParticipantService from '@/services/GameParticipantService';
import RoomParticipantService from '@/services/RoomParticipantService';
import { defineComponent } from 'vue';

export default defineComponent({
  name: "GameParticipant",
  data() {
    return {
      GameStateType,
      QuizQuestionType,
      
      roomService: new RoomParticipantService(),
      gameService: new GameParticipantService(),

      roomInfo: null as RoomInfo,
      gameState: null as GameState,

      answer: "" as string,
      answerOptionInd: null as number,
      answered: false as boolean,

      isGameRunning: null as boolean,
      isInitialized: false as boolean,

      stateRefreshTimer: null,
    };
  },
  computed: {
    questionOptions() {
      return this.gameState.question.options.map((o, ind) => ({
        title: o,
        value: ind,
      }));
    },
    canAnswer() {
      return this.gameState.question.type === QuizQuestionType.Text && this.answer.length > 0
        || this.gameState.question.type === QuizQuestionType.Choise && this.answerOptionInd !== null;
    },
  },
  methods: {
    async leaveRoom() {
      if (this.gameState.type !== GameStateType.Results)
        await this.roomService.leaveRoom(this.$route.params.id);

      router.push({ path: "/dashboard" });
    },
    async refreshIsGameRunning() {
      this.isGameRunning = await this.roomService.getIsGameRunning(this.$route.params.id);
      if (this.isGameRunning) {
        clearInterval(this.stateRefreshTimer);
        this.stateRefreshTimer = setInterval(() => this.refreshState(), 2000);
      }
    },
    async refreshState() {
      const prevStateType = this.gameState?.type;
      this.gameState = await this.gameService.getState(this.$route.params.id);
      if (prevStateType !== this.gameState.type) {
        this.answered = false;

        if (this.gameState.type === GameStateType.Results)
          clearInterval(this.stateRefreshTimer);
      }
    },
    async doAnswer() {
      await this.gameService.answer(this.$route.params.id, {
        answer: this.answer,
        answerOptionInd: this.answerOptionInd,
      });

      this.answered = true;
    },
  },
  async mounted() {
    this.roomInfo = await this.roomService.getInfo(this.$route.params.id);
    this.isGameRunning = await this.roomService.getIsGameRunning(this.$route.params.id);
    if (this.isGameRunning)
      await this.refreshState();

    this.isInitialized = true;

    if (this.isGameRunning)
      this.stateRefreshTimer = setInterval(() => this.refreshState(), 2000);
    else
      this.stateRefreshTimer = setInterval(() => this.refreshIsGameRunning(), 2000);
  },
  beforeUnmount() {
    clearInterval(this.stateRefreshTimer);
  },
});
</script>