<template>
  <div class="ma-auto" v-if="!isInitialized">
    <v-progress-circular indeterminate size="x-large"/>
  </div>
  <div v-else>
    <v-container max-width="1000px">
      <v-card>
        <v-card-title>
          <div class="mb-1 d-flex align-center">
            <v-btn
              class="mr-3"
              icon="mdi-arrow-left"
              variant="text"
              @click="back"
            />
            <div>Настройки комнаты</div>
            <div v-if="roomState.id" class="ml-auto d-flex align-center">
              <div>ID: {{ roomState.id }}</div>
              <v-btn
                class="ml-3"
                icon="mdi-content-copy"
                size="x-small"
                variant="outlined"
                @click="copyID"
              />
            </div>
          </div>
          <v-divider thickness="2"/>
        </v-card-title>
        <v-card-text style="padding-bottom: 0;">
          <v-text-field
            v-model="roomConfig.info.name"
            label="Name"
            :rules="[notEmptyRule]"
          />
          <v-text-field
            v-model="roomConfig.info.description"
            label="Description"
          />
          <v-select
            v-model="roomConfig.quizId"
            label="Quiz"
            :items="quizSelect"
            :rules="[notEmptyRule]"
          />
          <v-text-field
            v-model="roomConfig.maxParticipants"
            type="number"
            label="Max participants"
          />
        </v-card-text>
        <v-card-actions>
          <v-btn
            v-if="!roomState.isOpen"
            class="ml-auto"
            variant="flat"
            prepend-icon="mdi-door-open"
            text="Открыть"
            :disabled="!isValid"
            @click="openRoom"
          />
          <v-btn
            v-if="roomState.isOpen"
            class="ml-auto"
            variant="flat"
            prepend-icon="mdi-content-save"
            text="Обновить"
            :disabled="!isValid"
            @click="updateConfig"
          />
          <v-btn
            v-if="roomState.isOpen"
            variant="outlined"
            color="error"
            prepend-icon="mdi-close"
            text="Закрыть"
            @click="closeRoom"
          />
        </v-card-actions>
      </v-card>
    </v-container>
    <v-container
      v-if="roomState.isOpen"
      max-width="1000px"
    >
      <v-card>
        <v-card-title>
          Участники
        </v-card-title>
        <v-card-text>
          <div v-if="!roomState.participants.length">
            Нет участников
          </div>
          <div v-else class="d-flex flex-wrap">
            <v-card
              v-for="participant in roomState.participants"
              class="ma-1"
              width="250"
            >
              <v-card-title class="d-flex justify-space-between align-center">
                <div style="overflow-x: hidden; text-overflow: ellipsis;">{{ participant }}</div>
                <v-btn
                  class="ml-2"
                  color="error"
                  icon="mdi-close"
                  size="small"
                  variant="outlined"
                  @click="kickParticipant(participant)"
                />
              </v-card-title>
            </v-card>
          </div>
        </v-card-text>
      </v-card>
    </v-container>
    <v-container
      v-if="roomState.isOpen"
      max-width="1000px"
      class="pt-0 d-flex justify-end"
    >
      <v-btn
        text="Начать"
        :disabled="roomState.participants.length === 0"
        @click="startGame"
      />
    </v-container>
  </div>
</template>

<script lang="ts">
import RoomConfig from '@/models/room/RoomConfig';
import UserData from '@/models/UserData';
import RoomAdminService from '@/services/RoomAdminService';
import UserService from '@/services/UserService';
import { defineComponent } from 'vue';
import router from '@/router';
import RoomState from '@/models/room/RoomState';
import { notEmptyRule } from '@/utils/rules';
import GameAdminService from '@/services/GameAdminService';

export default defineComponent({
  name: "Room",
  data() {
    return {
      notEmptyRule,

      userService: new UserService(),
      roomService: new RoomAdminService(),
      gameService: new GameAdminService(),

      userData: null as UserData,
      roomConfig: null as RoomConfig,
      roomState: null as RoomState,

      isInitialized: false as boolean,

      stateRefreshTimer: null,
    };
  },
  computed: {
    quizSelect() {
      return this.userData.quizzes.map(q => ({
        title: q.name,
        value: q.id,
      }));
    },
    isValid() {
      return notEmptyRule(this.roomConfig.info.name) === true
        && this.roomConfig.quizId
        && notEmptyRule(this.roomConfig.quizId) === true;
    },
  },
  methods: {
    back() {
      router.push({ path: "/dashboard" });
    },
    copyID() {
      navigator.clipboard.writeText(this.roomState.id);
    },
    async refreshState() {
      this.roomState = await this.roomService.getState();
    },
    async updateConfig() {
      this.roomConfig.maxParticipants = Number(this.roomConfig.maxParticipants);
      await this.roomService.updateConfig(this.roomConfig);
    },
    async openRoom() {
      await this.updateConfig();
      await this.roomService.openRoom();
      await this.refreshState();

      this.stateRefreshTimer = setInterval(() => this.refreshState(), 2000);
    },
    async closeRoom() {
      await this.roomService.closeRoom();
      await this.refreshState();

      clearInterval(this.stateRefreshTimer);
      this.stateRefreshTimer = null;
    },
    async kickParticipant(participant: string) {
      await this.roomService.kickParticipant(participant);
    },
    async startGame() {
      await this.gameService.startGame();

      router.push({ path: "/game" });
    },
  },
  beforeUnmount() {
    if (this.stateRefreshTimer)
      clearInterval(this.stateRefreshTimer);
  },
  async mounted() {
    this.userData = await this.userService.getUserData();
    this.roomConfig = await this.roomService.getConfig();
    if (!this.userData.quizzes.map(q => q.id).includes(this.roomConfig.quizId))
      this.roomConfig.quizId = null;

    this.roomState = await this.roomService.getState();

    if (this.roomState.open)
      this.stateRefreshTimer = setInterval(() => this.refreshState(), 2000);

    this.isInitialized = true;
  },
});
</script>