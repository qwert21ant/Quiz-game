<template>
  <div class="ma-auto" v-if="!isInitialized">
    <v-progress-circular indeterminate size="x-large"/>
  </div>
  <div v-else>
    <v-container max-width="1000px">
      <v-card>
        <v-card-title class="d-flex">
          <div>Комната {{ roomConfig.info.name }}</div>
          <v-btn
            class="ml-auto"
            color="error"
            variant="outlined"
            prepend-icon="mdi-close"
            text="Прекратить игру"
            @click="endGame"
          />
        </v-card-title>
        <v-card-text>
          {{ roomConfig.info.description ?? "Без описания" }}
        </v-card-text>
      </v-card>
    </v-container>
    <v-container
      v-if="gameState.type == GameStateType.Question || gameState.type == GameStateType.Answer"
      max-width="1000px"
    >
      <v-card>
        <v-card-title>Ответы участников</v-card-title>
        <v-card-text>
          <v-card
            v-for="answer in participantsAnswers"
            class="ma-1"
            width="250"
          >
            <v-card-title class="d-flex justify-space-between align-center">
              <div style="overflow-x: hidden; text-overflow: ellipsis;">{{ answer.participant }}: {{ answer.answer }}</div>
              <v-icon
                class="ml-2"
                :color="isAnswerCorrect(answer.answer) ? 'success' : 'error'"
                :icon="isAnswerCorrect(answer.answer) ? 'mdi-check' : 'mdi-close'"
                size="small"
                variant="plain"
              />
            </v-card-title>
          </v-card>
        </v-card-text>
      </v-card>
    </v-container>
    <v-container max-width="1000px">
      <v-card
        v-if="gameState.type == GameStateType.Waiting"
      >
        <v-card-title>Выберите следующий вопрос</v-card-title>
        <v-card-text class="pb-0">
          <v-select
            v-model="nextQuestionInd"
            label="Следующий вопрос"
            :items="quizQuestions"
          />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
            text="Продолжить"
            variant="elevated"
            :disabled="nextQuestionInd == null"
            @click="nextQuestion"
          />
        </v-card-actions>
      </v-card>
      <div
          v-if="gameState.type == GameStateType.Question"
          class="d-flex flex-column align-center"
        >
          <v-progress-circular indeterminate size="x-large"/>
          <div class="mt-3">Ожидание ответов участников</div>
        </div>
      <div v-if="gameState.type == GameStateType.Answer">
        
        <v-card>
          <v-card-title>Выберите следующий вопрос</v-card-title>
          <v-card-text class="pb-0">
            <v-select
              v-model="nextQuestionInd"
              label="Следующий вопрос"
              :items="quizQuestions"
            />
          </v-card-text>
          <v-card-actions>
            <v-spacer />
            <v-btn
              text="Продолжить"
              variant="elevated"
              @click="nextQuestion"
            />
          </v-card-actions>
        </v-card>
      </div>
      <div v-if="gameState.type == GameStateType.Results"></div>
    </v-container>
  </div>
</template>

<script lang="ts">
import GameAnswer from '@/models/game/GameAnswer';
import GameState from '@/models/game/GameState';
import GameStateType from '@/models/game/GameStateType';
import Quiz from '@/models/quiz/Quiz';
import QuizQuestionType from '@/models/quiz/QuizQuestionType';
import RoomConfig from '@/models/room/RoomConfig';
import router from '@/router';
import GameAdminService from '@/services/GameAdminService';
import QuizService from '@/services/QuizService';
import RoomAdminService from '@/services/RoomAdminService';
import { defineComponent } from 'vue';

export default defineComponent({
  name: "GameAdmin",
  data() {
    return {
      GameStateType,

      roomService: new RoomAdminService(),
      gameService: new GameAdminService(),
      quizService: new QuizService(),

      roomConfig: null as RoomConfig,
      gameState: null as GameState,
      quiz: null as Quiz,
      
      nextQuestionInd: null as number,

      isInitialized: false as boolean,

      stateRefreshTimer: null,
    };
  },
  computed: {
    quizQuestions() {
      return this.quiz.questions.map((q, ind) => ({
        title: q.text,
        value: ind,
      }));
    },
    participantsAnswers() {
      return Object.entries<GameAnswer>(this.gameState.participantsAnswers).map(entry => ({
        participant: entry[0],
        answer: entry[1].answer == null ? entry[1].answerOptionInd : entry[1].answer,
      }));
    },
  },
  methods: {
    isAnswerCorrect(answer: string | number) {
      return this.gameState.question.type === QuizQuestionType.Text && this.gameState.answer.answer === answer
        || this.gameState.question.type === QuizQuestionType.Choise && this.gameState.answer.answeroptionInd === answer;
    },
    async endGame() {
      await this.gameService.endGame();

      router.push({ path: "/room" });
    },
    async refreshState() {
      const prevStateType = this.gameState?.type;
      this.gameState = await this.gameService.getState();
      if (prevStateType != this.gameState.type) {
        this.nextQuestionInd = this.gameState.nextQuestionInd;

        if (this.gameState.type !== GameStateType.Question)
          clearInterval(this.stateRefreshTimer);
      }
    },
    async nextQuestion() {
      await this.gameService.selectNextQuestion(this.nextQuestionInd);
      await this.gameService.nextQuestion();

      await this.refreshState();
      this.stateRefreshTimer = setInterval(() => this.refreshState(), 2000);
    },
  },
  async mounted() {
    this.roomConfig = await this.roomService.getConfig();
    this.quiz = await this.quizService.getQuiz(this.roomConfig.quizId);
    await this.refreshState();

    this.isInitialized = true;
  },
  beforeUnmount() {
    clearInterval(this.stateRefreshTimer);
  },
});
</script>