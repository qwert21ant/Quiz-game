<template>
  <div class="ma-auto" v-if="!userData">
    <v-progress-circular indeterminate size="x-large"/>
  </div>
  <div v-else>
    <v-container max-width="1000px">
      <v-card>
        <v-card-title>Управление комнатой</v-card-title>
        <v-card-text>
          <v-btn
            class="mr-3"
            prepend-icon="mdi-door"
            text="Моя комната"
            @click="enterMyRoom"
          />
          <v-btn
            prepend-icon="mdi-location-enter"
            color="secondary"
            text="Войти по коду"
            @click="showRoomEntranceDialog = true"
          />
        </v-card-text>
      </v-card>
    </v-container>
    <v-container max-width="1000px">
      <v-card>
        <v-card-title class="d-flex justify-space-between">
          <div>
            Мои квизы
          </div>
          <v-btn
            prepend-icon="mdi-plus"
            @click="showQuizCreationDialog = true"
          >
            Создать
          </v-btn>
        </v-card-title>
        <v-card-text>
          <div v-if="!userData.quizzes.length">
            У вас нет квизов
          </div>
          <div v-else class="d-flex flex-wrap">
            <v-card
              v-for="quiz in userData.quizzes"
              class="ma-1"
              width="250"
              @click="openQuiz(quiz.id)"
            >
              <v-card-title>{{ quiz.name }}</v-card-title>
            </v-card>
          </div>
        </v-card-text>
      </v-card>
    </v-container>
    <RoomEntranceDialog
      v-model="showRoomEntranceDialog"
      @join="joinRoom"
    />
    <QuizCreationDialog
      v-model="showQuizCreationDialog"
      @create="createQuiz"
    />
  </div>
</template>

<script lang="ts">
import UserData from '@/models/UserData';
import router from '@/router';
import UserService from '@/services/UserService';
import { defineComponent } from 'vue';
import RoomEntranceDialog from './RoomEntranceDialog.vue';
import RoomParticipantService from '@/services/RoomParticipantService';
import QuizCreationDialog from './QuizCreationDialog.vue';
import QuizService from '@/services/QuizService';

export default defineComponent({
  name: "Dashboard",
  components: { RoomEntranceDialog, QuizCreationDialog },
  data() {
    return {
      userService: new UserService(),
      roomService: new RoomParticipantService(),
      quizService: new QuizService(),

      userData: null as UserData,

      showRoomEntranceDialog: false as boolean,
      showQuizCreationDialog: false as boolean,
    };
  },
  methods: {
    enterMyRoom() {
      router.push({ path: "/room" });
    },
    async joinRoom(roomId: string) {
      await this.roomService.joinRoom(roomId);

      router.push({ path: "/game/" + roomId });
    },
    async createQuiz(quizName: string) {
      const id = await this.quizService.createQuiz(quizName);
      router.push({ path: "/quiz/" + id });
    },
    openQuiz(quizId: string) {
      router.push({ path: "/quiz/" + quizId });
    },
  },
  async mounted() {
    this.userData = await this.userService.getUserData();
  },
});
</script>
